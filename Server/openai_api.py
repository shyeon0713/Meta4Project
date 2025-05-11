import openai
import os
from dotenv import load_dotenv

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")

def ask_gpt(user_input: str , model: str = "gpt-4o-mini") -> str:
    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": "너의 이름은 수노이고, 이주 전 행방불명된 언니가 있어."},
            {"role": "user", "content": user_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content