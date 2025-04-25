using System;
using UnityEngine;

[Serializable]
public class JsonReader   //JSON ������ �Ľ� Ŭ���� 
{
    public string speaker;
    public string text;
    public string sprite;  //ĳ���� ǥ��
}
[Serializable]
public class DialogueData
{
    public JsonReader[] dialogues;

    public static DialogueData LoadFromJson(string path)
    {
        string json = System.IO.File.ReadAllText(path);  //������ ��°�� �о ���ڿ�(string)�� ��ȯ (System.io.file.readalltext)
        return JsonUtility.FromJson<DialogueData>(json);  //JSON ���ڿ��� C# Ŭ���� ��ü�� ��ȯ
    }
}