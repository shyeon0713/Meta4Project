from pydantic import BaseModel  #Pydantic을 이용한 데이터 유효성 검사 모델

# 데이터 검증 스키마
# log용
class DialogueBase(BaseModel):
    day: int #day 추가
    speaker: str  #SQLAlchemy에서는 text타입이 존재하지만 Pydantic(BaseModel)에서는 text타입이 존재하지 않는다.
    line: str
    likeability: float  #호감도 추가
    affection_change: float #호감도 변화 정도 추가
    class Config:
        orm_mode = True





# save용 ====================================================================
# 입력용 (생성/수정시 스키마)
class SaveBase(BaseModel):  #클라(유니티)에서 서버로 데이터를 보낼 때 사용하는 스키마라 Config필요 x
    slot_number: int
    day: int
    likeability: float
    last_dialogue_id: int
    last_speaker: str
    last_line: str


# save 조회시 출력용 스키마 (필요한 필드만 추출)
class SaveOutBase(BaseModel):  #서버에서 클라로 보낼때는 json변형이 필요하기 때문에 config넣음
    slot_number: int
    day: int
    likeability: float
    last_speaker: str
    class Config:
        orm_mode = True



# 기존에 있던 save 덮어쓰기 시 사용하는 스키마
class SaveUpdateBase(BaseModel):  #slot_number 제외한 업데이트 스키마
    day: int
    likeability: float
    last_dialogue_id: int
    last_speaker: str
    last_line: str

#