from sqlalchemy import Column, Integer, String, Float, Text, ForeignKey, JSON
from .database import Base


# log db
class Dialogue(Base):
    __tablename__ = "dialogue"

    id = Column(Integer, primary_key=True, autoincrement=True)
    day = Column(Integer, default= 1) #day 추가
    speaker = Column(String(100))
    line = Column(Text)
    likeability = Column(Float, default= 2.5)  #호감도 추가
    affection_change = Column(Float, default= 0) #호감도 변화 정도 추가

# save db
class Save(Base):
    __tablename__ = "save"

    primary_key = Column(Integer, primary_key=True, autoincrement=True)
    slot_number = Column(Integer, unique=True, nullable=False)  #슬롯 번호 저장 칼럼 추가

    day = Column(Integer)
    likeability = Column(Float)
    last_dialogue_id = Column(Integer, ForeignKey('dialogue.id'))  #dialogue id 가지고 옴
    last_speaker = Column(String(100))
    last_line = Column(Text)
