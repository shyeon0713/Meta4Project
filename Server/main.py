from fastapi import FastAPI, HTTPException, Depends, status
from pydantic import BaseModel
from typing import Annotated
from . import models
from .database import engine, SessionLocal
from sqlalchemy.orm import Session
from typing import Optional

# from api import npc_router

app = FastAPI()
models.Base.metadata.create_all(bind=engine)

# 의존성 주입(Dependency Injection)을 위한 함수
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

db_dependency = Annotated[Session, Depends(get_db)]



# API 라우터 등록
# npc.router - npc.py에서 정의한 APIRouter 객체를 등록 (등록해야 FastAPI 앱에서 사용 가능)
# prefix="/npc" - 모든 엔드포인트 앞에 /npc를 자동으로 추가
# tags=["NPC"] - FastAPI 자동 문서 (/docs)에서 이 API 그룹의 태그를 설정
# app.include_router(npc.router, prefix="/npc", tags=["NPC"])





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





# api
# 헬스체크 API
@app.get("/ping")
async def ping():
    return {"message": "Server is running"}