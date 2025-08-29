using System;

[Serializable]
public class Savedata
{
    public int primary_key;  // 대사 인덱스
    public int day;   //현재 Day
    public int likeability;  //현재 호감도
    public string last_dialogue_id;  //마지막 대사 인덱스 -> save용
    public string last_speaker;  //마지막 발화자 ->save 용
    public string last_line;  //마지막 대사
}