using UnityEngine;
using System.Collections;

public class SaveSlotsManager : MonoBehaviour
{// 전체 저장 슬롯 관리 및 저장/로드 로직 호출
    [Header("Slot UI Instances (Index 순서대로),전체 저장 슬롯 관리 및 저장/로드 로직 호출")]
    public SaveSlotUI[] slotUIs;

    private SaveSlotsData saveData;

    private void Start()
    {
        // 1) 기존 저장 불러오기
        saveData = DataManager.Load();

        // 2) 슬롯 UI 초기화 및 데이터 표시
        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].Init(i, OnSlotSelected);
            var file = saveData.slots.Length > i ? saveData.slots[i] : null;
            slotUIs[i].Populate(file);
        }
    }

    private void OnSlotSelected(int index) // 슬롯 클릭 시 호출
    {
        // 저장
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
