from fastapi import APIRouter, FastAPI, HTTPException, Depends, status
from sqlalchemy.orm import Session
from Server import models
from typing import List
from Server.schema import DialogueBase, SaveBase
from Server.database import db_dependency  #의존성 주입
from Server.openai_api import ask_gpt  #open ai api 가지고 오기

from typing import Optional  # 첫 날 save없을 경우. 해당 인자는 정수(or 다른 지정한 것) int일수도 있고, 없으면 None일 수도 있다는 것.


# 라우터임을 명시, 라우터임을 선언
router = APIRouter()

# 새로시작 버튼을 눌렀을 경우의 처음 정해진 player의 대사 전송 및 저장 api
@router.post("/dialogue/start", status_code=status.HTTP_201_CREATED)
async def newGame_start(db: db_dependency):

    player_line = "(달려가서 수노의 손목을 잡고) 저기 잠시만요! 물어보고 싶은것이 있어요."

    # 플레이어의 답변 db저장용
    db_user = models.Dialogue(
        day = 1,  # day추가
        speaker="player",
        line=player_line
    )
    db.add(db_user)
    db.commit()
    db.refresh(db_user)  # ID 확인용 / 데이터베이스에서 다시 조회하여 최신 값으로 db_user 객체를 업데이트


    # GPT에게 전송 (응답까지 저장)
    answer = ask_gpt(player_line)


    db_llm = models.Dialogue(
        day = 1,  # day추가
        speaker="suno",
        line=answer
    )
    db.add(db_llm)
    db.commit()
    db.refresh(db_llm)


    # 그냥 응답 확인용 return
    return {
        "user_id": db_user.id,
        "llm_id": db_llm.id,
        "response": answer
    }




# 플레이어 입력 받아 db저장 api (새로시작 후 or 이어하기 시)
@router.post("/dialogue/", status_code=status.HTTP_201_CREATED)
async def create_dialogue(dialogue:DialogueBase, db: db_dependency, save_id: Optional[int] = None):

    # save_id가 있으면 해당 세이브의 day를 사용, 없으면 현재 dialogue 테이블에서 최대 day 사용
    if save_id:
        save_state = db.query(models.Save).filter(models.Save.primary_key == save_id).first()
        if not save_state:
            raise HTTPException(status_code=404, detail="Save not found")
        current_day = save_state.day
    else:
        # 세이브 없이 새로시작 후 진행하는 경우 - 현재 dialogue에서 최대 day 찾기
        last_dialogue = db.query(models.Dialogue).order_by(models.Dialogue.id.desc()).first()
        current_day = last_dialogue.day if last_dialogue else 1


    # 플레이어의 답변 db저장용
    db_user = models.Dialogue(
        day = current_day,  # day추가
        speaker="player",
        line=dialogue.line
    )
    db.add(db_user)
    db.commit()
    db.refresh(db_user)  # ID 확인용 / 데이터베이스에서 다시 조회하여 최신 값으로 db_user 객체를 업데이트


    # 여기 사이에 gpt에게 이전 대화나 정보들을 주어서 일관성을 유지하도록 할 수 있는 코드가 추가적으로 필요함.


    # GPT에게 전송 (응답까지 저장)
    answer = ask_gpt(dialogue.line)


    db_llm = models.Dialogue(
        day = current_day,  # day추가
        speaker="suno",
        line=answer
    )
    db.add(db_llm)
    db.commit()
    db.refresh(db_llm)



    # 그냥 응답 확인용 return
    return {
        "user_id": db_user.id,
        "llm_id": db_llm.id,
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



# save 저장하기 api 추가 필요



# save 불러오기 (이어하기 눌렀을 경우)
# 클라이언트가 저장 목록에서 save_id 선택
# 해당 세이브 정보를 불러오고 → UI 및 state를 복원





# log 읽기 api (이건 대사 하나만 읽는 코드래...)
'''
@app.get("/dialogue/{dialogue_id}", response_model=DialogueBase, status_code=status.HTTP_200_OK)
async def read_dialogue(dialogue_id: int, db: db_dependency):
    dialogue = db.query(models.Dialogue).filter(models.dialogue.id == dialogue_id).first()
    if dialogue is None:
        raise HTTPException(status_code=404, detail='Dialogue not found')
    return dialogue
'''