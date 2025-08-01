from fastapi import APIRouter, FastAPI, HTTPException, Depends, status
from sqlalchemy.orm import Session
from Server import models
from typing import List
from Server.schema import DialogueBase, SaveBase
from Server.database import db_dependency  #의존성 주입
from Server.openai_api import ask_gpt_with_context  #open ai api 가지고 오기

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
    answer = ask_gpt_with_context(player_line)

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
# ============================================================================================



# 플레이어 입력 받아 db저장 api (새로시작 후 or 이어하기 시)
@router.post("/dialogue/", status_code=status.HTTP_201_CREATED)
async def create_dialogue(dialogue:DialogueBase, db: db_dependency, save_id: Optional[int] = None):

    # save_id가 있으면 해당 세이브의 day를 사용, 없으면 현재 dialogue 테이블에서 최대 day 사용
    if save_id:
        save_state = db.query(models.Save).filter(models.Save.primary_key == save_id).first()
        if not save_state:
            raise HTTPException(status_code=404, detail="Save not found")
        current_day = save_state.day
        saved_likeability = save_state.likeability if hasattr(save_state, 'likeability') else 2.5  #hasattr - save_state 객체에 'likeability'라는 속성(attribute) 이 있는지 확인
    else:
        # 세이브 없이 새로시작 후 진행하는 경우 - 현재 dialogue에서 최대 day 찾기
        last_dialogue = db.query(models.Dialogue).order_by(models.Dialogue.id.desc()).first()
        current_day = last_dialogue.day if last_dialogue else 1
        saved_likeability = None  #나중에 계산


    # 플레이어의 답변 db저장용
    db_user = models.Dialogue(
        day = current_day,  #day추가
        speaker="player",
        line=dialogue.line
    )
    db.add(db_user)
    db.commit()
    db.refresh(db_user)  # ID 확인용 / 데이터베이스에서 다시 조회하여 최신 값으로 db_user 객체를 업데이트


    
    # 과거 대화한 대사들을 가지고옴 (최신부터 6개, 객체리스트로 반환)
    dialogue_history = (
        db.query(models.Dialogue)
        .filter(models.Dialogue.day == current_day)
        .order_by(models.Dialogue.id.desc())
        .limit(6)
        .all()
    )

    # 현재 호감도 결정 (아직 시기상조, 과거 대사가지고오는것부터 완료해야함)
    '''
    if save_id:
        # 세이브 불러온 경우: 세이브된 호감도 사용
        current_likeability = saved_likeability
    else:
        # 새로시작 후 연속진행: dialogue 테이블에서 가장 최근 호감도 찾기
        current_likeability = get_latest_affection_from_dialogue(db, current_day)
    '''


    current_likeability = saved_likeability  #일단 오류 안나게 임시방편

    # GPT에게 전송 (응답, 현 day, 이전 대화, 현 호감도)
    # GPT의 대답을 가지고 옴
    answer = ask_gpt_with_context(dialogue.line, current_day, dialogue_history, current_likeability)


    # 호감도 변화
    new_likeability = current_likeability


    # llm의 답변 db저장
    db_llm = models.Dialogue(
        day = current_day,  # day추가
        speaker="suno",
        line=answer,
        likeability=new_likeability
    )
    db.add(db_llm)
    db.commit()
    db.refresh(db_llm)



    # day종료조건 판단하는 함수를 불러와야함. (dayCheck.py import -> 딕셔너리 반환)
    # 만약 day조건을 다 달성했다면, 수노가 마지막 대사를 뱉었는지(day 완료 조건을 만족하는지) 점검해야함.
    # day조건 달성 + 수노 마지막 대사 뱉었음 -> 그럼 다음 day로 넘어가도록함.


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