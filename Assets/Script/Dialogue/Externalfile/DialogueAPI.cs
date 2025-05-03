using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class DialogueAPI : MonoBehaviour
{
    private const string API_URL = "fastapi sever URL";  //서버 URL넣기
    public IEnumerator SendPlayerReply(string playerInput, Action<string> callback)
    {
        var json = JsonUtility.ToJson(new PlayerReply { reply = playerInput });

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("API Error: " + request.error);
            }
        }
    }

    [Serializable]
    public class PlayerReply
    {
        public string reply;
    }
}
