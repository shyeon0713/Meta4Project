using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

[Serializable]
[Tooltip("save 서버에 반환")]
public class ServerState   
{
    public int day;
    public int likeability;
    public int last_dialogue_id;
    public string last_speaker;
    public string last_line;
}

public class Save_api : MonoBehaviour
{
    public static Save_api Instance { get; private set; }

    private const string STATE_URL = "http://127.0.0.1:8000/save";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public IEnumerator GetServerState(Action<ServerState> onSuccess, Action<string> onError)
    {
        const string url = "http://127.0.0.1:8000/save"; 
        using var req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var state = JsonUtility.FromJson<ServerState>(req.downloadHandler.text);
                onSuccess?.Invoke(state);
            }
            catch (Exception ex)  // 파싱 부분 오류
            {
                onError?.Invoke($"JSON 파싱 오류: {ex.Message}");
            }
        }
        else  //네트워크 오류
        {
            onError?.Invoke($"네트워크 오류: {req.error} (Code:{req.responseCode})");
        }
    }
}

