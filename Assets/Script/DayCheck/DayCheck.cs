using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class SaveData
{
    public int day;      
    public string location;     
}

[System.Serializable]
public struct DayLocationSprite
{
    public int day;
    public string locationName;
    public Sprite checkday;
    public Sprite background;
}


public class DayCheck : MonoBehaviour
{
    [Header("API Settings")]   //API 서버 세팅 부분 -> 로컬서버 연결 후에 진행해야함 제발제발까먹지말아요 제발제발
    [Tooltip("FastAPI에서 세이브 데이터를 가져올 URL (GET)")]
    //Tooltip : 유니티 에디터의 인스펙터 창에서 해당 필드 위에 마우스를 올렸을 때 작은 설명 창(툴팁)을 띄워 주는 역할을 함 -> 주석 비슷하게 활용
    public string apiUrl = "http://127.0.0.1:8000/savefile";

    [Header("Scene References")]
    [Tooltip("daycheck 표시할 UI Image 컴포넌트")]
    public Image daygroundImage;

    [Header("Scene References")]
    [Tooltip("background 표시할 UI Image 컴포넌트")]
    public Image backgroundImage;

    [Header("Day-Location to Sprite Mapping")]
    [Tooltip("요일, 장소에 따라 배경 이미지를 설정할 데이터 리스트")]
    public DayLocationSprite[] scenelist;

    private SaveData currentSave;

    private void Start()
    {
        // 초기 데이터 로드
        StartCoroutine(FetchSaveData());
    }

    private IEnumerator FetchSaveData()
    {
        using (var request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                currentSave = JsonUtility.FromJson<SaveData>(request.downloadHandler.text);
                ApplySceneSettings();
            }
            else  // 테스트 후 주석 처리
            {
                Debug.LogError($"SaveData fetch failed: {request.error}");
            }
        }
    }

    private void ApplySceneSettings()
    {
        Debug.Log($"Day: {currentSave.day}, Location: {currentSave.location}");
        foreach (var entry in scenelist)
        {
            if (entry.day == currentSave.day && entry.locationName == currentSave.location)
            {
                backgroundImage.sprite = entry.background;
                return;
            }
        }
    }

    public void AdvanceDay()
    {
        // 다음 날로 이동
        currentSave.day = (currentSave.day + 1) % 7;
        ApplySceneSettings();
    }

}

