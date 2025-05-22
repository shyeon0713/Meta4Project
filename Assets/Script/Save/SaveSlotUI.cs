using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text dayText;
    public Button slotButton;

    [HideInInspector]
    public int slotIndex;
    private Action<int> onSlotSelected;

    [Tooltip("5개의 Heart Image")]
    public Image[] hearts;


    public void Init(int index, Action<int> onSelected) // 외부에서 초기화
    {
        slotIndex = index;
        onSlotSelected = onSelected;
        slotButton.onClick.AddListener(() => onSlotSelected?.Invoke(slotIndex));
    }

    public void Populate(SaveFile data)// 전달받은 저장 데이터로 UI와 하트를 갱신
    {
        if (data == null)
        {
            dayText.text = "0";
            UpdateHearts(0f);
        }
        else
        {
            dayText.text = $"Day: {data.day}";
            UpdateHearts(data.likeability);
        }
    }


    public void UpdateHearts(float value)//likeability 값에 따라 하트 이미지를 채우기
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            float fill = Mathf.Clamp01(value - i);
            hearts[i].fillAmount = fill;
        }
    }

    // 에디터에서 hearts 배열이나 likeability 값을 변경할 때 즉시 반영용
    private void OnValidate()
    {
        if (hearts != null && hearts.Length > 0)
            UpdateHearts(0f);
    }
}

