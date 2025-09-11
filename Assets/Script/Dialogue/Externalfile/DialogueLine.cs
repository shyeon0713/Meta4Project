using System;

[Serializable]
public class DialogueLine
{
    public string speaker;
    public string line;    // ← 서버가 보내는 키 이름과 일치시키기
    public int day;
    public float affection_change;
    public float likeability;
}

[Serializable]
public class CreateDialogueResponse
{
    public int user_id;
    public int llm_id;
    public string speaker;
    public string response;
    public int day;
    public float affection_change;
    public float likeability;
}