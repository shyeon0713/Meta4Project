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

    [Tooltip("5���� Heart Image")]
    public Image[] hearts;


    public void Init(int index, Action<int> onSelected) // �ܺο��� �ʱ�ȭ
    {
        slotIndex = index;
        onSlotSelected = onSelected;
        slotButton.onClick.AddListener(() => onSlotSelected?.Invoke(slotIndex));
    }

    public void Populate(SaveFile data)// ���޹��� ���� �����ͷ� UI�� ��Ʈ�� ����
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


    public void UpdateHearts(float value)//likeability ���� ���� ��Ʈ �̹����� ä���
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            float fill = Mathf.Clamp01(value - i);
            hearts[i].fillAmount = fill;
        }
    }

    // �����Ϳ��� hearts �迭�̳� likeability ���� ������ �� ��� �ݿ���
    private void OnValidate()
    {
        if (hearts != null && hearts.Length > 0)
            UpdateHearts(0f);
    }
}

