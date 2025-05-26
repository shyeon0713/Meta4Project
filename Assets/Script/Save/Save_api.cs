using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;



public class Save_api : MonoBehaviour
{
    public static Save_api Instance { get; private set; }

    private const string Post_url = "http://127.0.0.1:8000/save";  //post요청
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()  
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   //중복 방지
        }
        else Destroy(gameObject);
    }

    public IEnumerator GetServerState(Action<SaveFile> onSuccess, Action<string> onError)
    {  
        const string Get_url = "http://127.0.0.1:8000/save/all/";   //Get 요청
        using var req = UnityWebRequest.Get(Get_url);
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                var save = JsonUtility.FromJson<SaveFile>(req.downloadHandler.text);
                onSuccess?.Invoke(save);
            }
            catch (Exception ex)  // 파싱 부분 오류
            {
                onError?.Invoke($"JSON 파싱 오류: {ex.Message}");
            }
        }
        else  //Get 오류일 경우
        {
            onError?.Invoke($"Get 오류: {req.error} (Code:{req.responseCode})");
        }

    }
        public IEnumerator PostServerState(SaveFile data, Action onSuccess, Action<string> onError = null)
        {   //json 직렬화
            string json = JsonUtility.ToJson(data);
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

            using var req = new UnityWebRequest(Post_url, "POST");
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                onError?.Invoke($"Post 오류{req.error} (HTTP {req.responseCode})");
            else
                onSuccess?.Invoke();
        }
    }



