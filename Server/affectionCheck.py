# 코드 기반 기본 호감도 변화 계산 (베이스라인)
def calculate_affection_change_baseline(player_input: str) -> float:
    affection_change = 0.0
    player_input_lower = player_input.lower()  #이거 대문자도 소문자랑 같이 취급한다 이런건데 일단 인풋이 한국어라 필요한지는 모르겠듬
    
    # 호감도 증가 조건들
    # 언니를 칭찬하는 경우 (+0.5)
    praise_keywords = ["예쁘", "아름다", "좋았", "사랑했", "소중했", "특별했", "멋있", "훌륭했", "착했", "따뜻했"]
    if any(keyword in player_input_lower for keyword in praise_keywords):
        affection_change += 0.5
    
    # 언니를 그리워하는 경우 (+1.0)
    miss_keywords = ["그리워", "보고싶", "그립", "그리운", "생각나", "떠올", "그때가", "돌아왔으면"]
    if any(keyword in player_input_lower for keyword in miss_keywords):
        affection_change += 1.0
    
    # 기억을 되찾으려 노력하는 경우 (+0.5)
    memory_keywords = ["기억", "생각해보", "떠올려", "생각나", "기억나", "회상", "그때", "노력", "애써"]
    if any(keyword in player_input_lower for keyword in memory_keywords):
        affection_change += 0.5
    
    # 언니의 특징을 언급하는 경우 (+0.5)
    detail_keywords = ["검은 머리", "단발", "고양이", "날카로운", "눈", "바다", "강릉", "밥", "웃음"]
    if any(keyword in player_input_lower for keyword in detail_keywords):
        affection_change += 0.5
    
    # 진실 추론/수노가 언니라는 것을 깨달은 경우 (+1.0)
    realization_keywords = ["동생", "닮았", "비슷", "똑같", "언니같", "그녀와"]
    if any(keyword in player_input_lower for keyword in realization_keywords):
        affection_change += 1.0
    



    # 호감도 감소 조건들
    # 비난조나 의심스러운 어조 (-0.5)
    accusatory_keywords = ["거짓말", "속이", "숨기", "의심", "믿을 수 없", "이상해", "수상", "뭔가"]
    if any(keyword in player_input_lower for keyword in accusatory_keywords):
        affection_change -= 0.5
    
    # 성급하거나 불신하는 경우 (-0.5)
    impatient_keywords = ["빨리", "당장", "그만", "됐어", "필요없", "상관없"]
    if any(keyword in player_input_lower for keyword in impatient_keywords):
        affection_change -= 0.5
    
    # 질문 회피 (-1.0)
    avoidance_keywords = ["모르겠", "기억 안", "말하기 싫", "상관없", "관심없", "알 필요"]
    if any(keyword in player_input_lower for keyword in avoidance_keywords):
        affection_change -= 1.0
    
    # 기억 회복에 무관심 (-1.0)
    disinterest_keywords = ["상관없어", "중요하지 않", "어쩔 수 없", "포기", "됐어"]
    if any(keyword in player_input_lower for keyword in disinterest_keywords):
        affection_change -= 1.0
    
    # 무신경하거나 사려깊지 못한 말 (-1.0)
    thoughtless_keywords = ["그냥", "별로", "뭐 어때", "상관없지", "아무래도"]
    if any(keyword in player_input_lower for keyword in thoughtless_keywords):
        affection_change -= 1.0
    
    # 욕설 사용 (-1.5)
    profanity_keywords = ["씨발", "시발", "개새끼", "병신", "미친", "좆", "엿", "닥쳐"]
    if any(keyword in player_input_lower for keyword in profanity_keywords):
        affection_change -= 1.5
    
    # 수노를 밀어내려는 시도 (-1.0)
    push_away_keywords = ["가", "나가", "떠나", "돌아가", "그만 와", "보지 말", "꺼져"]
    if any(keyword in player_input_lower for keyword in push_away_keywords):
        affection_change -= 1.0
    
    return affection_change
