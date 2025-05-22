from fastapi import APIRouter, FastAPI, HTTPException, Depends, status
from sqlalchemy.orm import Session
from Server import models
from typing import List
from Server.schema import DialogueBase, SaveBase
from Server.database import db_dependency  #의존성 주입
from Server.openai_api import ask_gpt  #open ai api 가지고 오기


# 라우터임을 명시, 라우터임을 선언
router = APIRouter()




# 플레이어 입력 받아 db저장 api
@router.post("/dialogue/", status_code=status.HTTP_201_CREATED)
async def create_dialogue(dialogue:DialogueBase, db: db_dependency):

    # 플레이어의 답변 db저장용
    db_user = models.Dialogue(
        speaker="나",
        line=dialogue.line
    )
    db.add(db_user)
    db.commit()
    db.refresh(db_user)  # ID 확인용 / 데이터베이스에서 다시 조회하여 최신 값으로 db_user 객체를 업데이트


    # 수노 응답 개수 세기 (이건 무조건 db가 일단 비워져있어야 함)
    me_reply = db.query(models.Dialogue).filter(models.Dialogue.speaker == "나").count()

    # GPT에게 전송 (응답까지 저장)
    answer = ask_gpt(dialogue.line, me_reply + 1)

    print(me_reply)


    db_llm = models.Dialogue(
        speaker="수노",
        line=answer
    )
    db.add(db_llm)
    db.commit()
    db.refresh(db_llm)



    # 그냥 응답 확인용 return
    return {
        "user_id": db_user.id,
        "llm_id": db_llm.id,
        "speaker":  db_llm.speaker,
        "response": answer
    }


# open ai test api
@router.get("/dialogue/{dialogue_id}")
async def bring_dialogue(id: int, db: db_dependency):
    llm_line = db.query(models.Dialogue).filter(models.Dialogue.id == id).first()
    if not llm_line:
        raise HTTPException(status_code=404, detail="Dialogue not found")
    return {
        "speaker": llm_line.speaker,
        "line": llm_line.line
        }






# log버튼 눌렀을 시 기존에 쌓인 로그들 불러오는 api
# log (누적 50개) 읽기 api
@router.get("/dialogue/logs", response_model=list[DialogueBase], status_code=status.HTTP_200_OK)
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
@router.post("/save/", status_code=status.HTTP_201_CREATED)
async def create_save(save:SaveBase, db: db_dependency):
    db_save = models.Save(**save.dict())
    db.add(db_save)
    db.commit()
    db.refresh(db_save)  #새로 생성된 id포함하여 리턴
    return db_save

# save (모두) 읽기 api
@router.get("/save/all", response_model=list[SaveBase], status_code=status.HTTP_200_OK)
async def read_save(db: db_dependency):
    save = (
        db.query(models.Save)
        .order_by(models.Save.primary_key.asc())
        .all()  #결과가 없다면 항상 빈 리스트 반환
    )
    return save




# log 읽기 api (이건 대사 하나만 읽는 코드래...)
'''
@app.get("/dialogue/{dialogue_id}", response_model=DialogueBase, status_code=status.HTTP_200_OK)
async def read_dialogue(dialogue_id: int, db: db_dependency):
    dialogue = db.query(models.Dialogue).filter(models.dialogue.id == dialogue_id).first()
    if dialogue is None:
        raise HTTPException(status_code=404, detail='Dialogue not found')
    return dialogue
'''