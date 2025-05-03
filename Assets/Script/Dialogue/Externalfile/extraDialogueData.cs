using UnityEngine;
using System;

[Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public string sprite;
}

[Serializable]
public class extraDialogueData
{
    public DialogueLine[] dialoglines;

    public static extraDialogueData extraFromJson(string json)
    {
        return JsonUtility.FromJson<extraDialogueData>(json);
    }
}

