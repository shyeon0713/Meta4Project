DAY_OBJECTIVE = """
[Progress – Day 1]

- Date: Day 1  
- Location: In front of the player’s house  
- Mood:  
  A girl who looks strikingly similar to the woman from the player’s recurring dreams appears.  
  The player feels compelled to talk to her. Suno does not refuse when the player approaches.

- What both characters know so far:
  - Suno believes the player knows everything about the incident.
  - The player knows nothing yet.

- Today’s dialogue goals:
  - Suno realizes the player has memory loss.  
  - The player learns Suno’s name.  
  - The player learns that Suno has an older sister.  
  - The player realizes he was romantically involved with Suno’s sister.  
  - The player finds out he went on a trip with her.

- Condition to move to Day 2:
  - If all of the above dialogue goals are completed, Suno must say:  
    “Let’s meet tomorrow at 2pm in the park. I’ll be going now.”
  - Then the story transitions to Day 2.

- If the goals are not fully achieved, Suno does **not** leave and continues to ask further questions or observe the player.


[Starting Line – Day 1 Trigger]
The player will **always begin** the conversation with the following line:  
> "Hey… there’s something I need to ask you. Can you spare me a moment?"
"""


# Day 1의 목표들 - 한글/영어 키워드 모두 포함
DAY_GOALS = {
    "memory_loss_realized": {
        "description": "Suno realizes the player has memory loss",
        "korean_keywords": ["기억", "잊다", "모르겠다", "생각나지", "까먹다", "기억나지"],
        "context_indicators": ["아무것도", "모르겠어", "어리둥절", "혼란스러", "이상해", "뭔가", "왜"],
        "suno_response_patterns": ["기억하지", "잊어버린", "모르는", "까먹은", "생각나지 않는", "기억을 잃은", "모르는군요"]
    },
    "player_learns_name": {
        "description": "Player learns Suno's name",
        "korean_keywords": ["이름", "누구", "이름이", "부르면"],
        "context_indicators": ["처음", "소개", "뭐라고", "불러", "누구세요"],
        "suno_response_patterns": ["제 이름", "이름은", "불러", "수노입니다", "수노예요", "제 이름은 수노"]
    },
    "learns_about_sister": {
        "description": "Player learns Suno has an older sister",
        "korean_keywords": ["언니", "누나", "자매", "가족", "동생"],
        "context_indicators": ["집안", "형제", "가족", "혼자", "누구"],
        "suno_response_patterns": ["언니", "누나", "자매", "우리 언니", "내 언니", "언니가"]
    },
    "romantic_involvement": {
        "description": "Player realizes romantic involvement with sister",
        "korean_keywords": ["사랑", "연인", "사귀", "좋아했", "연애", "애인", "남친", "여친"],
        "context_indicators": ["둘이서", "데이트", "커플", "관계", "특별한"],
        "suno_response_patterns": ["사랑했", "사귀었", "연인", "좋아했", "애인"]
    },
    "trip_mentioned": {
        "description": "Player finds out about the trip",
        "korean_keywords": ["여행", "여행갔", "어디갔", "같이갔", '놀러갔', "다녀왔"],
        "context_indicators": ["떠났", "함께", "어디", "갔었", "없었"],
        "suno_response_patterns": ["여행", "갔었", "다녀왔", "같이 갔", "놀러"]
    }
}

DAY_1_COMPLETION_PHRASE = "오늘은 일단 여기까지 하죠. 내일 오후 2시에 공원에서 만나요. 이만 가볼게요."