from pydantic import BaseModel  #Pydantic을 이용한 데이터 유효성 검사 모델

# 데이터 검증 스키마
# log용
class DialogueBase(BaseModel):
    speaker: str  #SQLAlchemy에서는 text타입이 존재하지만 Pydantic(BaseModel)에서는 text타입이 존재하지 않는다.
    line: str
    class Config:
        orm_mode = True

# save용
class SaveBase(BaseModel):
    day: int
    likeability: float
    last_dialogue_id: int
    last_speaker: str
    last_line: str
    class Config:
        orm_mode = True