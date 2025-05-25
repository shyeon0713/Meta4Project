using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DayLocationSprite
{
    public int day;           // Day 번호
    public Sprite checkday;   // 체크 스프라이트
    public Sprite background; // 배경 스프라이트
}

public class DayCheck : MonoBehaviour
{
    [Header("UI References")]
    public Image daycheck;         // 체크 이미지
    public Button dayCheckButton;   // 체크 클릭 버튼
    public Image background;       // 배경 이미지

    [Header("Day → Sprite Mapping")]
    public DayLocationSprite[] scenelist;

    private SaveFile currentSave;
    private bool initialPosted = false;
    private int currentDay;
    public int CurrentDay
    {
        get { return currentDay; }
    }

    void Start()
    {
        // UI 모두 숨기기
        daycheck.gameObject.SetActive(false);
        dayCheckButton.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        // 체크 클릭 리스너
        dayCheckButton.onClick.AddListener(OnDayCheckClicked);

        // 서버에서 현재 세이브 불러오기
        StartCoroutine(
            Save_api.Instance.GetServerState(
                onSuccess: OnGetSuccess,
                onError: OnGetError
            )
        );
    }

    void OnGetSuccess(SaveFile save)
    {
        currentSave = save;

        // 처음 실행 시 초기값 세팅 필요
        if (save.day < 1 && !initialPosted)
        {
            currentSave.day = 1;
            currentSave.likeability = 0;
            currentSave.last_dialogue_id = 0;
            currentSave.last_speaker = string.Empty;
            currentSave.last_line = string.Empty;

            StartCoroutine(
                Save_api.Instance.PostServerState(
                    currentSave,
                    onSuccess: () =>
                    {
                        initialPosted = true;
                        currentDay = 1;
                        ShowDayCheck();
                    },
                    onError: err =>
                    {
                        Debug.LogError("초기값 POST 실패: " + err);
                        currentDay = 1;
                        ShowDayCheck();
                    }
                )
            );
        }
        else
        {
            // 기존 저장된 Day 사용
            currentDay = currentSave.day;
            ShowDayCheck();
        }
    }

    void OnGetError(string err)
    {
        Debug.LogWarning("세이브 로드 실패: " + err);
        currentDay = 1;
        ShowDayCheck();
    }

    /// <summary>
    /// 체크 스프라이트만 표시
    /// </summary>
    void ShowDayCheck()
    {
        foreach (var entry in scenelist)
        {
            if (entry.day == currentDay)
            {
                daycheck.sprite = entry.checkday;
                daycheck.gameObject.SetActive(true);
                dayCheckButton.gameObject.SetActive(true);
                return;
            }
        }
        Debug.LogWarning($"Day {currentDay}에 매핑된 체크 이미지가 없습니다.");
    }

    /// <summary>
    /// 체크 클릭 시 Day 배경 표시
    /// </summary>
    void OnDayCheckClicked()
    {
        daycheck.gameObject.SetActive(false);
        dayCheckButton.gameObject.SetActive(false);

        foreach (var entry in scenelist)
        {
            if (entry.day == currentDay)
            {
                background.sprite = entry.background;
                background.gameObject.SetActive(true);
                break;
            }
        }
    }

    /// <summary>
    /// AIUI에서 호출: 다음 Day로 저장 및 UI 갱신
    /// </summary>
    public IEnumerator AdvanceDayAndSave(int nextDay)
    {
        // 다음 Day로 증가
        currentSave.day = nextDay;

        // 기존 필드(호감도, 대사 등)는 currentSave에 보존됨

        yield return StartCoroutine(
            Save_api.Instance.PostServerState(
                currentSave,
                onSuccess: () =>
                {
                    currentDay = currentSave.day;
                    background.gameObject.SetActive(false);
                    ShowDayCheck();
                },
                onError: err => Debug.LogError("Save POST 실패: " + err)
            )
        );
    }
}
