import openai
import os
from dotenv import load_dotenv
import Server.suno as suno
import Server.day_prompts.day_1 as day_1

load_dotenv()
openai.api_key = os.getenv("OPENAI_API_KEY")


#reply_count 없에야함
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