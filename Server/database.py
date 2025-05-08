from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base

# //루트:비밀번호@로컬호스트:포트번호/데이터베이스이름
# 만약 비밀번호 안에 @,!가 포함이 되어있다면 기호들을 인코딩 해야한다
# @ -> %40, ! -> %21
URL_DATABASE = 'mysql+pymysql://root:test1234!@localhost:3306/BlogApplication'

# engine 객체를 생성하면 SQLAlchemy가 해당 데이터베이스와 연결을 설정하고 SQL 명령을 실행할 수 있도록 한다.
engine = create_engine(URL_DATABASE)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

Base = declarative_base()