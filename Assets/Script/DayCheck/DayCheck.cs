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
    [Header("API Settings")]   //API ���� ���� �κ� -> ���ü��� ���� �Ŀ� �����ؾ��� �������߱�������ƿ� ��������
    [Tooltip("FastAPI���� ���̺� �����͸� ������ URL (GET)")]
    //Tooltip : ����Ƽ �������� �ν����� â���� �ش� �ʵ� ���� ���콺�� �÷��� �� ���� ���� â(����)�� ��� �ִ� ������ �� -> �ּ� ����ϰ� Ȱ��
    public string apiUrl = "http://127.0.0.1:8000/savefile";

    [Header("Scene References")]
    [Tooltip("daycheck ǥ���� UI Image ������Ʈ")]
    public Image daygroundImage;

    [Header("Scene References")]
    [Tooltip("background ǥ���� UI Image ������Ʈ")]
    public Image backgroundImage;

    [Header("Day-Location to Sprite Mapping")]
    [Tooltip("����, ��ҿ� ���� ��� �̹����� ������ ������ ����Ʈ")]
    public DayLocationSprite[] scenelist;

    private SaveData currentSave;

    private void Start()
    {
        // �ʱ� ������ �ε�
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
            else  // �׽�Ʈ �� �ּ� ó��
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
        // ���� ���� �̵�
        currentSave.day = (currentSave.day + 1) % 7;
        ApplySceneSettings();
    }

}

