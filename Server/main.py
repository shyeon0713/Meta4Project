from fastapi import FastAPI, HTTPException, Depends, status
from pydantic import BaseModel
from typing import Annotated
import models
from database import engine, SessionLocal
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
    likeability: float
    line: str
    last_speaker: str
    last_line: str
    class Config:
        orm_mode = True





# api
# 헬스체크 API
@app.get("/ping")
async def ping():
    return {"message": "Server is running"}


# log
# log 생성 api
@app.post("/dialogue/", status_code=status.HTTP_201_CREATED)
async def create_dialogue(dialogue:DialogueBase, db: db_dependency):
    db_dialogue = models.CharacterInfo(**dialogue.dict())
    db.add(db_dialogue)
    db.commit()

# log 읽기 api (하단 쭉 수정필요)
@app.get("/dialogue/{NPC_name}", response_model=CharacterInfoBase, status_code=status.HTTP_200_OK)
async def read_dialogue(NPC_name: str, db: db_dependency):
    characterInfo = db.query(models.CharacterInfo).filter(models.CharacterInfo.NPC_name == NPC_name).first()
    if characterInfo is None:
        raise HTTPException(status_code=404, detail='Character not found')
    return characterInfo



# save
# save 생성 api
@app.post("/ScenarioList/", status_code=status.HTTP_201_CREATED)
async def create_scenario(scenarioList:ScenarioListBase, db: db_dependency):
    db_scenario = models.ScenarioList(**scenarioList.dict())
    db.add(db_scenario)
    db.commit()

# save 읽기 api
@app.get("/ScenarioList/{scenario_name}", response_model=ScenarioListBase, status_code=status.HTTP_200_OK)
async def read_scenario(scenario_name: str, db: db_dependency):
    scenario = db.query(models.ScenarioList).filter(models.ScenarioList.scenario_name == scenario_name).first()
    if scenario is None:
        raise HTTPException(status_code=404, detail='Scenario not found')
    return scenario



# 대사
# 대사 정보 생성 api
@app.post("/ScenarioInfo/", status_code=status.HTTP_201_CREATED)
async def create_scenarioInfo(scenarioInfo:ScenarioInfoBase, db: db_dependency):
    db_scenarioInfo = models.ScenarioInfo(**scenarioInfo.dict())
    db.add(db_scenarioInfo)
    db.commit()

# 대사 정보 읽기 api (시나리오 이름 뿐만 아니라 뭔가를 하나 더받아야할것같아. 순서 번호같은거... 근데 str임 int가 아닌데?)
@app.get("/ScenarioInfo/{scenario_name}", response_model=ScenarioInfoBase, status_code=status.HTTP_200_OK)
async def read_scenarioInfo(scenario_name: str, db: db_dependency):
    scenarioInfo = db.query(models.ScenarioInfo).filter(models.ScenarioInfo.scenario_name == scenario_name).first()
    if scenarioInfo is None:
        raise HTTPException(status_code=404, detail='ScenarioInfo not found')
    return scenarioInfo


