import openai
import os
from dotenv import load_dotenv
import Server.suno as suno

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")

def ask_gpt(player_input: str , model: str = "gpt-4o-mini") -> str:
    response = openai.ChatCompletion.create(
        model=model,
        messages=[
            {"role": "system", "content": suno.SUNO_SYSTEM_PROMPT},
            {"role": "user", "content": player_input}
        ],
        temperature=0.8
    )
    return response.choices[0].message.content