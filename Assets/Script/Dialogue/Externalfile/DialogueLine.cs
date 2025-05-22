using System;

[Serializable]
public class DialogueLine
{
    public string speaker;
    public string line;    // �� ������ ������ Ű �̸��� ��ġ��Ű��
}

[Serializable]
public class CreateDialogueResponse
{
    public int user_id;
    public int llm_id;
    public string speaker;
    public string response;
}