using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[System.Serializable]
public class PlayerReply  //플레이어 답변 전달
{
    public string line;
    public string speaker;

    public PlayerReply(string line, string speaker)
    {
        this.line = line;
        this.speaker = speaker;
    }
}

public class DialogueAPI : MonoBehaviour
{
    private const string API_URL = "http://127.0.0.1:8000/dialogue/";  //���� URL�ֱ�

    public DialogueLine savescript;

    public IEnumerator SendPlayerReply(string line, string speaker)
    {

        PlayerReply data = new PlayerReply(line, speaker);       // 서버변수 = 내가 쓸변수,
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<CreateDialogueResponse>(request.downloadHandler.text);
            savescript = new DialogueLine
            {
                speaker = resp.speaker,   // 서버가 고정으로 보내주지않음
                line = resp.response 
            };
        }
        else
        {
            Debug.LogError($"API Error: {request.error} | Code: {request.responseCode}");
            Debug.LogError($"Response Body: {request.downloadHandler.text}");
        }
    }
}
