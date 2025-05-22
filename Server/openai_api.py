import openai
import os
from dotenv import load_dotenv
import Server.suno as suno
import Server.day_prompts.day_1_test as dayt_1

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")

def ask_gpt(player_input: str , reply_count: int, model: str = "gpt-4o-mini") -> str:


    if reply_count == 5:
        reply_instruction = (
        "\n\n[Instruction for GPT]\n"
        "This is the 6th and final player question for today.\n"
        "You MUST end your response with the following exact line:\n"
        "\"더 궁금한게 있다면 내일 2시 공원에서 보죠. 기다리고 있을게요요.\"\n"
        "No extra dialogue should come after this line."
    )
    else:
        reply_instruction = ""


    reply_count = f"\n\n[System Info for GPT]\nCurrent player question count: {reply_count}\n"
    
    system_prompt = f"{suno.SUNO_SYSTEM_PROMPT}\n\n{dayt_1.DAY_OBJECTIVE}\n\n[System Info for GPT]\nCurrent player question count: {reply_count}{reply_instruction}"

    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content