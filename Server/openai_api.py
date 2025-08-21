import openai
import os
from dotenv import load_dotenv
import Server.suno as suno
from Server.dayCheck import check_day_goals
from Server.affectionCheck import calculate_affection_change_baseline
import re  #정규표현식 모듈. llm응답에서 정해진 패턴을 찾아서 숫자만 추출 (re.search(패턴, 텍스트))

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")


# 일단 이건 맥락없이 단순 gpt 호출 (게임 진행 중, 우리가 이전에 입력했던 말을 기억하지 못함.)
# 게임 시작 맨 처음 start api에서 사용. (그럼 이제 서로 주고 받고 하나의 세트만이 완성될 것.)
def ask_gpt(player_input: str, model: str = "gpt-4o-mini") -> str:    
    system_prompt = f"{suno.SUNO_SYSTEM_PROMPT}"

    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content
# =========================================================================================




# 각 데이의 마지막 대사 가지고오는 함수
def get_day_completion_phrase(day: int) -> str:
    try:
        if day == 1:
            from Server.day_prompts.day_1 import DAY_1_COMPLETION_PHRASE
            return DAY_1_COMPLETION_PHRASE

        #추후 추가 (하단에 예시)        
        
        else:
            return "오늘은 여기까지입니다."
    except ImportError:
        return "오늘은 여기까지입니다."



# 과거의 몇 문장을 가지고와서 대사 입력하고 전송시 같이 전송하여 문맥을 gpt가 파악할 수 있도록 한다.
# 호감도와 수노의 응답을 반환 (tuple[float, str])
def ask_gpt_with_context(player_input: str, day: int, dialogue_history: list, current_affection: float, goals_achieved:dict, model: str = "gpt-4o-mini") -> tuple[float, float, str]:
    # 과거 대화들을 가지고옴
    conversation_context = ""
    for dialogue in dialogue_history:
        if dialogue.speaker == "player":
            conversation_context += f"Player: {dialogue.line}\n"
        else:
            conversation_context += f"Suno: {dialogue.line}\n"
    
    

    # 아직 여기 day체크 미추가 (목표 달성 여부 딕셔너리를 같이 넘겨서 프롬프팅 해야할듯.)
    # llm이 목표달성여부 파악해서 마지막대사를 자연스럽게 말할 수 있도록한다.
    
    # 데이 목표를 가지고온다 (현재 목표 달성 상태 계산)
    goals_achieved = check_day_goals (day, dialogue_history, goals_achieved)

    # 데이 마지막 대사들 가지고옴
    completion_phrase = get_day_completion_phrase(day)

    # day goals 달성 상태 text (이건 gpt에게 달성상태를 전해주는 용도)
    goals_status_text = "Current Goal Achievement Status:\n"
    for goal, achieved in goals_achieved.items():  #딕셔너리(dict)에서 (key, value) 쌍을 하나씩 꺼내주는 함수
        status = "Achieved" if achieved else "Not Achieved"
        goals_status_text += f"- {goal}: {status}\n"

    # 모든 목표 달성 여부 확인 (딕셔너리 값의 모든 목표들이 true일때만 all_goals_achieved = true)
    if goals_achieved:
        all_goals_achieved = all(goals_achieved.values()) #딕셔너리 값들만의 모음
    else:
        all_goals_achieved = False



    # 호감도 판단을 여기서 해야함.
    # 가지고 온 이전 대화들을 넘겨주면서, 1차 코드로 판단, 2차 llm이 대화 분위기보며 판단.
    # 그래서 여기에 호감도 관련 판단 코드필요 (프롬프트로 같이 넘기려면)
    
    # 일단 코드로 호감도 먼저 판단
    baseline_change = calculate_affection_change_baseline(player_input)
    
    # 호감도 상태 설명
    if current_affection >= 4.5:
        affection_description = "Feels very trusting and warm"
    elif current_affection >= 4.0:
        affection_description = "Begins to trust and let go of hostility"
    elif current_affection >= 3.5:
        affection_description = "Stops speaking harshly and starts to believe the player might not have killed her sister. Begins to show respect."
    elif current_affection >= 2.0:
        affection_description = "Suspicious of the player"
    else:
        affection_description = "Very distrustful and cold"


    
    # 보낼 시스템 프롬프트
    system_prompt = f"""{suno.SUNO_SYSTEM_PROMPT}

    Conversation so far (most recent):{conversation_context}

    Current goal completion status:{goals_status_text}

    **CRITICAL DAY COMPLETION RULE**: 
    If ALL goals for Day {day} are achieved ({all_goals_achieved}), you MUST end your response with this EXACT phrase:
    "{completion_phrase}"

    Current affection level: {current_affection:.1f}/5.0 ({affection_description})

    ADDITIONAL INSTRUCTIONS:
    Code analysis suggests affinity change: {baseline_change:+.1f}

    Please consider:
    - Overall tone and sincerity of the player
    - Context that code analysis might miss (sarcasm, deeper meaning, etc.)
    - Emotional weight of the conversation
    - How this fits with Suno's personality and current state

    You must respond as Suno, then add this line:
    [AffinityChange: X.X] 

    Consider both the code suggestion ({baseline_change:+.1f}) and your own contextual judgment based on the Affinity System rules above.
    If they align, use similar values. If context suggests different, adjust accordingly.
    Range: -2.0 to +2.0 per interaction.

    Day 7 ending condition: If day={day} AND affection>=4.5 AND truth mostly understood, use ending phrases from the Ending Trigger section.
    """

    # 메시지 구성 개선  (시스템 프롬프트, 각자 이때까지 말했던 대사, 현재 플레이어의 대사 3개가 messages에 담김)
    messages = [{"role": "system", "content": system_prompt}]
    
    # dialogue_history를 role에 맞게 추가
    for dialogue in dialogue_history:
        role = "user" if dialogue.speaker == "player" else "assistant"
        messages.append({"role": role, "content": dialogue.line})
    
    # 현재 플레이어 입력 추가
    messages.append({"role": "user", "content": player_input})


    # llm에 프롬프트와 함께 플레이어의 입력 전송
    response = openai.ChatCompletion.create(
        model=model,
        messages=messages,
        temperature=0.8,
        frequency_penalty=0.5, # 같은 말 반복 억제
        presence_penalty=0.6   # 새로운 주제 유도
    )

    full_reply = response.choices[0].message.content  #수노의 답변을 반환


    # LLM의 호감도 변화 파싱
    match = re.search(r"\[AffinityChange:\s*([+-]?\d+(\.\d+)?)\]", full_reply)
    if match:
        llm_change = float(match.group(1))  #0: 정규표현식 전체, 1: 뒤에올 +0.5만 파싱, 2: .5의 소숫점만 파싱 (그걸 float로 소수로 바꿈)
        suno_reply = full_reply.replace(match.group(0), "").strip() #전체 대사에서 [AffinityChange: +0.5] 줄 제거 -> 수노의 대사만 들어가게 함
    else:
        # LLM이 형식을 지키지 않은 경우 베이스라인 사용
        llm_change = baseline_change #코드로 계산해둔 호감도 그냥 사용
        suno_reply = full_reply.strip()
        print(f"Warning: LLM didn't provide affinity change, using baseline: {baseline_change}")
    

    # 최종 호감도 계산
    # 코드와 LLM 판단의 가중평균 (코드 30%, LLM 70%)
    final_change = (baseline_change * 0.3) + (llm_change * 0.7)
    
    # 극단적인 변화 제한 (-2.0 ~ +2.0)
    final_change = max(-2.0, min(2.0, final_change))
    
    # 최종 호감도 적용
    new_affection = max(0.0, min(5.0, current_affection + final_change))
    
    # 디버깅용 로그
    print(f"Affection Debug - Baseline: {baseline_change:+.1f}, LLM: {llm_change:+.1f}, Final: {final_change:+.1f}, New: {new_affection:.1f}")



    return new_affection, final_change, suno_reply






'''
최근 대화 기록 (6줄 정도)

GPT에 보냄

GPT 응답 수신

조건 키워드 충족 여부 판단 (check_day_goals())

수노가 마지막 퇴장 문장 말했는지 확인 (check_day_completion_by_suno())

감정 변화 업데이트
'''


'''
        elif day == 2:
            from Server.day_prompts.day_2 import DAY_2_COMPLETION_PHRASE  
            return DAY_2_COMPLETION_PHRASE
        elif day == 3:
            from Server.day_prompts.day_3 import DAY_3_COMPLETION_PHRASE
            return DAY_3_COMPLETION_PHRASE 
'''