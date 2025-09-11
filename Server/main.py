from fastapi import FastAPI, HTTPException, Depends, status
from Server import models
from Server.database import engine
from sqlalchemy.orm import Session
from Server.router import router

# from api import npc_router

# 메인 실행 파일 -> app
app = FastAPI()
models.Base.metadata.create_all(bind=engine)


# 헬스체크 API
@app.get("/ping")
async def ping():
    return {"message": "Server is running"}




# 라우터 등록
# npc.router - npc.py에서 정의한 APIRouter 객체를 등록 (등록해야 FastAPI 앱에서 사용 가능)
# prefix="/npc" - 모든 엔드포인트 앞에 /npc를 자동으로 추가
# tags=["NPC"] - FastAPI 자동 문서 (/docs)에서 이 API 그룹의 태그를 설정
app.include_router(router, prefix="", tags=["API"])
