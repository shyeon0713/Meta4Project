import Server.day_prompts.day_1 as day_1

# day조건 판단 함수

def get_day_prompt(day: int) -> str:
    """Day별 프롬프트 반환"""
    day_prompts = {
        1: day_1.DAY_OBJECTIVE,
        # 2: day_2.DAY_OBJECTIVE,  # 추후 추가
        # 3: day_3.DAY_OBJECTIVE,  # 추후 추가
    }
    return day_prompts.get(day, day_1.DAY_OBJECTIVE)  # 기본값은 day_1


def check_day_completion(day: int, db) -> bool:
    """
    현재 day의 대화가 완료 조건을 만족하는지 확인
    """
    from . import models  # 순환 import 방지
    
    # 현재 day의 모든 대화 기록 가져오기
    dialogue_history = db.query(models.Dialogue)\
                        .filter(models.Dialogue.day == day)\
                        .order_by(models.Dialogue.id.desc())\
                        .all()
    
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
        completion_phrase = "Let's meet tomorrow at 2pm in the park. I'll be going now."
        return completion_phrase in latest_suno_line
    # elif day == 2:
    #     completion_phrase_2 = "다음 day 2 완료 문구"
    #     return completion_phrase_2 in latest_suno_line
    
    return False


