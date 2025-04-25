using System;
using UnityEngine;

[Serializable]
public class JsonReader   //JSON 데이터 파싱 클래스 
{
    public string speaker;
    public string text;
    public string sprite;  //캐릭터 표정
}
[Serializable]
public class DialogueData
{
    public JsonReader[] dialogues;

    public static DialogueData LoadFromJson(string path)
    {
        string json = System.IO.File.ReadAllText(path);  //파일을 통째로 읽어서 문자열(string)로 반환 (System.io.file.readalltext)
        return JsonUtility.FromJson<DialogueData>(json);  //JSON 문자열을 C# 클래스 객체로 변환
    }
}