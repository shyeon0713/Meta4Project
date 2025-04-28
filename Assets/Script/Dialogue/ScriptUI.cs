using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class ScriptUI : MonoBehaviour
{
    public Button nextbutton;  //버튼 활성화

    private DialogueData dialogueData;  // json에서 불러온 대사 데이터 참조를 위해
    public TMP_Text npcnametext;
    public TMP_Text dialogueText;

    public Image npcImage;  //NPC 이미지 
    public List<Sprite> npcSpriteList;
    public List<string> npcSpriteNames;

    float gray = 111f / 225f;  //내가 말할 경우, NPC 회색처리를 위해

    private Dictionary<string, Sprite> npcSpriteDict = new();

    private int currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextbutton.onClick.AddListener(OutputScript);  // 버튼 연결

        // JSON 파일 로드
        string path = Path.Combine(Application.streamingAssetsPath, "MainStory.json");

        // DialogueData 초기화
        dialogueData = DialogueData.LoadFromJson(path);  // `LoadFromJson` 사용!

        Debug.Log("Path to JSON: " + path);

        // 파일 경로 확인 및 데이터 검증
        if (dialogueData == null || dialogueData.dialogues == null || dialogueData.dialogues.Length == 0)
        {
            Debug.LogError("Dialogue data is empty or could not be loaded.");  //로드가 되지 않을 경우
            return;
        }

        // 스프라이트 딕셔너리 초기화
        if (npcSpriteList.Count != npcSpriteNames.Count)
        {
            Debug.LogError("Sprite list and sprite name list do not match in length!");
            return;
        }

        for (int i = 0; i < npcSpriteList.Count; i++)
        {
            npcSpriteDict[npcSpriteNames[i]] = npcSpriteList[i];
        }

        OutputDialogue(currentIndex);
    }


    void OutputScript()  // 다음 버튼 누르면 다음 대사 출력
    {
        currentIndex++;

        if (dialogueData == null || dialogueData.dialogues == null)  //0424수정 : null일 경우를 생각안함
        {
            Debug.LogError("Dialogue data or dialogues is null!");
            return;  // 더 이상 진행하지 않음
        }

        if (currentIndex < dialogueData.dialogues.Length)
        {
            OutputDialogue(currentIndex);
        }
        else  //마지막 대사 이후 처리
        {
         //  nextbutton.interactable = false;  // 버튼 비활성화
            dialogueText.text = ""; 
        }

    }


    void OutputDialogue(int index)
    {
        var line = dialogueData.dialogues[index];  // var: 컴파일러가 변수의 타입을 자동으로 추론하게 해주는 키워드
        npcnametext.text = line.speaker;
        dialogueText.text = line.text;

        npcImage.color = new Color(1f, 1f, 1f, 1f);  //기본색 -> 흰색

        if (line.speaker == "수노")  //수노가 말할 경우, 수노LD는 흰색
        {
            npcImage.sprite = GetSpriteFromDict(line.sprite);

        }
        else  // 내가 말할 경우, 수노LD는 회색
        {
            npcImage.sprite = GetSpriteFromDict(line.sprite);
            npcImage.color = new Color(gray, gray, gray, 1f); // 회색 처리
        }
    }

    Sprite GetSpriteFromDict(string spriteName) 
      //JSON에서 넘어온 sprite 이름을 실제 Unity의 Sprite 객체로 바꾸기
    {
        if (npcSpriteDict.TryGetValue(spriteName, out Sprite sprite))
        //TryGetValue : Dictionary에서 값이 존재하는지 안전하게 꺼내는 방법 + null 에러 방지
        //존재하면 값 반환, 없으면 에러 없이 처리 가능
        {
            return sprite;
        }
        return null;
    }

}
