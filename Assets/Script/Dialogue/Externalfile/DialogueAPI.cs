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
    public int day;
    public float affection_change;
    public float likeability;


    public PlayerReply(string line, string speaker,
        int day,float affection_change, float likeability)
    {
        this.line = line;
        this.speaker = speaker;
        this.day = day;
        this.likeability = likeability;
        this.affection_change = affection_change;
    }
}

public class DialogueAPI : MonoBehaviour
{
    private const string API_URL = "http://127.0.0.1:8000/dialogue/";  //���� URL�ֱ�

    public DialogueLine savescript;

    // 외부에서 접근 가능한 멤버 추가
    public int day;
    public float likeability;
    public float affection_change;

    #region 플레이어 처음대사 출력
    public IEnumerator GetFirstPlayerLine()
    {
        string url = API_URL;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var resp = JsonUtility.FromJson<CreateDialogueResponse>(request.downloadHandler.text);

            savescript = new DialogueLine
            {
                speaker = resp.speaker,
                line = resp.response,
                day = resp.day,
                likeability = resp.likeability,
                affection_change = resp.affection_change
            };

        }
        else  // api 연동이 제대로 되는지 확인
        {
            Debug.LogError("첫 대사 요청 실패" + request.error);
        }
    }

    #endregion



    #region 플레이어 응답보내는 메서드
    public IEnumerator SendPlayerReply(string line, string speaker, int day, float likeability, float affection_change)
            {

                PlayerReply data = new PlayerReply(line, speaker, day, affection_change, likeability);       // 서버변수 = 내가 쓸변수,
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

                    // 서버 응답 값을 DialogueAPI 필드에 저장
                    day = resp.day;
                    likeability = resp.likeability;
                    affection_change = resp.affection_change;

                    savescript = new DialogueLine
                    {
                        speaker = resp.speaker,   // 서버가 고정으로 보내주지않음
                        line = resp.response,
                        likeability = resp.likeability,
                        affection_change = resp.affection_change,
                        day = resp.day
                    };
                }
                else
                {
                    Debug.LogError($"API Error: {request.error} | Code: {request.responseCode}");
                    Debug.LogError($"Response Body: {request.downloadHandler.text}");
                }
            }
}

#endregion