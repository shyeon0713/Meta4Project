using UnityEngine;
using System.Collections;

public class SaveSlotsManager : MonoBehaviour
{// ��ü ���� ���� ���� �� ����/�ε� ���� ȣ��
    [Header("Slot UI Instances (Index �������),��ü ���� ���� ���� �� ����/�ε� ���� ȣ��")]
    public SaveSlotUI[] slotUIs;

    private SaveSlotsData saveData;

    private void Start()
    {
        // 1) ���� ���� �ҷ�����
        saveData = DataManager.Load();

        // 2) ���� UI �ʱ�ȭ �� ������ ǥ��
        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].Init(i, OnSlotSelected);
            var file = saveData.slots.Length > i ? saveData.slots[i] : null;
            slotUIs[i].Populate(file);
        }
    }

    private void OnSlotSelected(int index) // ���� Ŭ�� �� ȣ��
    {
        // ����
        StartCoroutine(Save_api.Instance.GetServerState(
            state =>
            {
                if (saveData.slots == null || saveData.slots.Length != DataManager.MaxSlots)
                    saveData.slots = new SaveFile[DataManager.MaxSlots];

                saveData.slots[index] = new SaveFile
                {
                    day = state.day,
                    likeability = state.likeability
                };

                DataManager.Save(saveData);
                slotUIs[index].Populate(saveData.slots[index]);
                Debug.Log($"Slot {index + 1} saved.");
            },
            err => Debug.LogError($"Save failed: {err}")
        ));
    }
}
