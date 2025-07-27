import openai
import os
from dotenv import load_dotenv
import Server.suno as suno
import Server.day_prompts.day_1 as day_1

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")


# 일단 이건 맥락없이 단순 gpt 호출 (게임 진행 중, 우리가 이전에 입력했던 말을 기억하지 못함.)
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





# 과거의 몇 문장을 가지고와서 대사 입력하고 전송시 같이 전송하여 문맥을 gpt가 파악할 수 있도록 한다.
def ask_gpt_with_context(player_input: str, day: int, dialogue_history: list, affection: float, model: str = "gpt-4o-mini") -> tuple[float, str]:
    # 과거 대화들을 가지고옴
    conversation_context = ""
    for dialogue in dialogue_history:
        if dialogue.speaker == "player":
            conversation_context += f"Player: {dialogue.line}\n"
        else:
            conversation_context += f"Suno: {dialogue.line}\n"
    

    #아직 여기 day체크 미추가
    system_prompt = f"""{suno.SUNO_SYSTEM_PROMPT}
    지금까지의 대화: {conversation_context}"""


    # 호감도 판단 정보도 필요해...


    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content





