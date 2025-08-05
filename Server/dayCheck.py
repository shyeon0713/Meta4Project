import Server.day_prompts.day_1 as day_1

# day조건 판단 함수



# day별 프롬프트 반환
def get_day_prompt(day: int) -> str:
    day_prompts = {
        1: day_1.DAY_OBJECTIVE,
        # 2: day_2.DAY_OBJECTIVE,  # 추후 추가
        # 3: day_3.DAY_OBJECTIVE,  # 추후 추가
    }
    return day_prompts.get(day, day_1.DAY_OBJECTIVE)  # 기본값은 day_1



# day별 목표(goal)반환 (일단필요없을듯)
'''
def get_day_goals(day: int):
    day_prompts = {
        1: day_1.DAY_GOALS,
        # 2: day_2.DAY_GOALS,  # 추후 추가
        # 3: day_3.DAY_GOALS,  # 추후 추가
    }
    return day_prompts.get(day, day_1.DAY_GOALS)  # 기본값은 day_1
    '''



# 목표 하나하나의 달성 여부 판단
# 해당 Day목표 달성 여부를 Boolen딕셔너리로 반환
def check_day_goals(day: int, dialogue_history: list):
    if day == 1:
        from Server.day_prompts.day_1 import DAY_GOALS
        goal_dict = DAY_GOALS
    else:
        return {}  # 또는 raise NotImplementedError

    goals_achieved = {goal: False for goal in goal_dict}


    for dialogue in dialogue_history:
        line = dialogue.line
        speaker = dialogue.speaker
        lower_line = line.lower()  #lower은 혹시모를 영어 입력의 대소문자 구분 없이 키워드 찾기 위함

        
        for goal_key, goal_data in goal_dict.items(): #.items() - 딕셔너리 키, 값 쌍얻기
            if goals_achieved[goal_key]:
                continue

            if speaker == "player":
                keywords = goal_data["korean_keywords"] + goal_data.get("context_indicators", [])
            elif speaker == "suno":
                keywords = goal_data.get("suno_response_patterns", [])  #goal_data 안에 "suno_response_patterns" 키가 항상 있지 않을 수도 있기 때문에 [] 넣어줌
            else:
                continue  # 불명확한 speaker는 스킵

            if any(kw in lower_line for kw in keywords):
                goals_achieved[goal_key] = True
        
    return goals_achieved





# 현재 day의 대화가 완료 조건을 만족하는지 확인
# 모든 목표가 달성이 되었고, llm에서 마지막 대사를 발화를 했는지 판단. -> true면 다음 day로 진행
# 완료유무를 판단하여 day를 넘기는 역할
def check_day_completion(day: int, dialogue_history: list) -> bool:
    # 대사 없으면 false
    if not dialogue_history:
        return False
    
    # 가장 최근 Suno의 대사 확인
    latest_suno_line = None
    for dialogue in dialogue_history:
        if dialogue.speaker == "suno":
            latest_suno_line = dialogue.line
            break
    
    if not latest_suno_line:
        return False
    
    # Day별 완료 조건 확인
    if day == 1:
        completion_phrase = "오늘은 일단 여기까지 하죠. 내일 오후 2시에 공원에서 만나요. 이만 가볼게요."
        return completion_phrase in latest_suno_line
    # elif day == 2:
    #     completion_phrase_2 = "다음 day 2 완료 문구"
    #     return completion_phrase_2 in latest_suno_line
    
    return False


