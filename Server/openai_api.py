import openai
import os
from dotenv import load_dotenv
import Server.suno as suno
from Server.dayCheck import check_day_goals

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")


# 일단 이건 맥락없이 단순 gpt 호출 (게임 진행 중, 우리가 이전에 입력했던 말을 기억하지 못함.)
# (일단필요없을듯)
'''
def ask_gpt(player_input: str, model: str = "gpt-4o-mini") -> str:    
    system_prompt = f"{suno.SUNO_SYSTEM_PROMPT}\n\n{day_1.DAY_OBJECTIVE}"

    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content
    '''





# 과거의 몇 문장을 가지고와서 대사 입력하고 전송시 같이 전송하여 문맥을 gpt가 파악할 수 있도록 한다.
# 호감도와 수노의 응답을 반환
def ask_gpt_with_context(player_input: str, day: int, dialogue_history: list, affection: float, model: str = "gpt-4o-mini") -> tuple[float, str]:
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
    goals_achieved = check_day_goals (day, dialogue_history)

    # day goals 달성 상태 text프롬프트
    goals_status_text = "Current Goal Achievement Status:\n"
    for goal, achieved in goals_achieved.items():  #딕셔너리(dict)에서 (key, value) 쌍을 하나씩 꺼내주는 함수
        status = "Achieved" if achieved else "Not Achieved"
        goals_status_text += f"- {goal}: {status}\n"



    # 호감도 판단을 여기서 해야함. (가지고 온 이전 대화들을 넘겨주면서)










    
    # 보낼 시스템 프롬프트
    system_prompt = f"""{suno.SUNO_SYSTEM_PROMPT}

    Conversation so far (most recent first): {conversation_context}

    Current goal completion status: {goals_status_text}
    
    If the player has achieved all the goals, Suno should say the final line to wrap up the day.

    """



    

    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content  #수노의 답변을 반환






'''
최근 대화 기록 (6줄 정도)

GPT에 보냄

GPT 응답 수신

조건 키워드 충족 여부 판단 (check_day_goals())

수노가 마지막 퇴장 문장 말했는지 확인 (check_day_completion_by_suno())

감정 변화 업데이트
'''