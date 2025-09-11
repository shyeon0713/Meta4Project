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
public class DayCheckUI : MonoBehaviour
{
    [Header("현재 DayCheck 배경")]
    public Image daycheck;         // 체크 이미지
    public Button dayCheckButton;   // 체크 클릭 버튼

    [Header("현재 배경")]
    public Image background;       // 배경 이미지 

    [Header("Day1부터 Day7까지의 리소스 리스트")]
    public DayLocationSprite[] scenelist;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private int currentDay;  // 현재 DAY -> 띄워야할 DAYCheck

    private void Start()
    {
        // 모든 리소스 false
        daycheck.gameObject.SetActive(false);
        dayCheckButton.gameObject.SetActive(false);
        background.gameObject.SetActive(false);

        // 체크 클릭 리스너
        dayCheckButton.onClick.AddListener(OnDayCheckClikced);  //화면 클릭 시, 리스너 추가

    }


    #region 화면에 DayCheck 이미지 출력
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

    #endregion

    #region 화면을 클릭할 경우, CheckDay 배경 사라지고 Background가 출력되는 메서드 -> OnDayCheckClicked

    public void OnDayCheckClikced() {

        SoundSetting.Instance.PlaySfx(8); // sfx8 -> book, 책넘기는 소리


        foreach (var entry in scenelist)
        {
            if (entry.day == currentDay)  // 현재day와 리스트의 있는 데이(인덱스)가 동일할 경우
            {
                background.sprite = entry.background;
                background.gameObject.SetActive(true);  // 배경이 화면에 출력
                break;
            }
        }

    }
    #endregion
}
