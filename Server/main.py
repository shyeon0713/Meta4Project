from fastapi import FastAPI, HTTPException, Depends, status
from pydantic import BaseModel
from typing import Annotated
from Server import models
from Server.database import engine, SessionLocal
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


# log
# log 생성 api
@app.post("/dialogue/", status_code=status.HTTP_201_CREATED)
async def create_dialogue(dialogue:DialogueBase, db: db_dependency):
    db_dialogue = models.Dialogue(**dialogue.dict())
    db.add(db_dialogue)
    db.commit()

# log 읽기 api (이건 대사 하나만 읽는 코드래...)
'''
@app.get("/dialogue/{dialogue_id}", response_model=DialogueBase, status_code=status.HTTP_200_OK)
async def read_dialogue(dialogue_id: int, db: db_dependency):
    dialogue = db.query(models.Dialogue).filter(models.dialogue.id == dialogue_id).first()
    if dialogue is None:
        raise HTTPException(status_code=404, detail='Dialogue not found')
    return dialogue
'''

# log (누적 50개) 읽기 api
@app.get("/dialogue/logs", response_model=list[DialogueBase], status_code=status.HTTP_200_OK)
async def read_dialogue(db: db_dependency):
    dialogue = (
        db.query(models.Dialogue)  #특정 테이블(모델)을 조회할 준비를 함.
        .order_by(models.Dialogue.id.asc())  #소문자 dialogue는 지역변수를 선언해야 쓸 수 있기 때문에 models.Dialogue로 정확하게 클래스 이름 사용해야함
        .limit(50)
        .all()  #-> 리스트 반환
        # 위의 매개변수 response_model=DialogueBase는 단일 객체 반환하는 걸로 인식하기 때문에 validationError 남. list[DialogueBase]로 바꾸어야 한다.
    )
    if dialogue is None:
        raise HTTPException(status_code=404, detail='Dialogue not found')
    return dialogue


# save
# save 생성 api
@app.post("/save/", status_code=status.HTTP_201_CREATED)
async def create_save(save:SaveBase, db: db_dependency):
    db_save = models.Save(**save.dict())
    db.add(db_save)
    db.commit()
    db.refresh(db_save)  #새로 생성된 id포함하여 리턴
    return db_save

# save (모두) 읽기 api
@app.get("/save/all", response_model=list[SaveBase], status_code=status.HTTP_200_OK)
async def read_save(db: db_dependency):
    save = (
        db.query(models.Save)
        .order_by(models.Save.primary_key.asc())
        .all()  #결과가 없다면 항상 빈 리스트 반환
    )
    return save