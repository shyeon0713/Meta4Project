from sqlalchemy import Column, Integer, String, Float, Text, ForeignKey, JSON
from database import Base


# log db
class Dialogue(Base):
    __tablename__ = "dialogue"

    primary_key = Column(Integer, primary_key=True, autoincrement=True)
    speaker = Column(String(100))
    line = Column(text)

# save db
class Save(Base):
    __tablename__ = "save"

    primary_key = Column(Integer, primary_key=True, autoincrement=True)
    likeability = Column(Float)
    last_speaker = Column(String(100))
    last_line = Column(text)
