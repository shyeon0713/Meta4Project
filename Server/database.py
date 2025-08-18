from fastapi import FastAPI, HTTPException, Depends, status
from typing import Annotated
from sqlalchemy.orm import Session
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base  #이동시 models/base.py내부
# from Server.password import URL_DATABASE

from dotenv import load_dotenv
import os

load_dotenv() #.env에 정의된 내용을 환경 변수로 등록
DATABASE_URL = os.getenv("DATABASE_URL") #등록된 환경변수 중 하나를 가져옴

# engine 객체를 생성하면 SQLAlchemy가 해당 데이터베이스와 연결을 설정하고 SQL 명령을 실행할 수 있도록 한다.
# 데이터베이스 연결 설정
engine = create_engine(DATABASE_URL)

# 세션 생성
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# ORM 모델 베이스 생성
Base = declarative_base()  #이동시 models/base.py내부


# 의존성 주입(Dependency Injection)을 위한 함수
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

db_dependency = Annotated[Session, Depends(get_db)]