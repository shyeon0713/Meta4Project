using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIUI : MonoBehaviour
{
    public TMP_Text speaker;// Name 표시

    // NPC 부분
    public TMP_Text NPCscript;
    public Button nextbutton; // 스크립트 넘김 버튼

    //플레이어 답변입력 부분
    public TMP_InputField PlayerInput;  //질문 입력
    public Button Inputbutton;  //입력 버튼
  
    public DialogueAPI dialogueService;

    private extraDialogueData currentDialogue;
    private int dialogueIndex = 0;

    [Header("Scene Controller")]
    public DayCheck sceneController;

    private void Start()
    {
        Inputbutton.onClick.AddListener(InputSend);
        nextbutton.onClick.AddListener(ShowNextScript);

        //플레이어 답변부분 숨기기
        Inputbutton.gameObject.SetActive(false);
        PlayerInput.gameObject.SetActive(false);

    }

    void InputSend()
    {
        string playerInput = PlayerInput.text;
        PlayerInput.text = "";

        //입력 후 다시 서버에 전송
        StartCoroutine(dialogueService.SendPlayerReply(playerInput, OnDialogueReceived));
    }

    void OnDialogueReceived(string json)  
    {
        currentDialogue = extraDialogueData.extraFromJson(json);  //extraDialogueData에서 참조
        dialogueIndex = 0; //임의로 인덱스를 int로 표시 
        if (currentDialogue.dialoglines != null && currentDialogue.dialoglines.Length > 0)
        {
            ShowCurrentScript();  
        }
    }
    void ShowCurrentScript()  //현재 화면에 표시되는 UI 설정
    {
        DialogueLine line = currentDialogue.dialoglines[dialogueIndex];

        // 기본적으로 모든 UI 숨김 -> 조건문으로 UI 꺼짐켜짐 설정
        speaker.gameObject.SetActive(false);
        NPCscript.gameObject.SetActive(false);
        PlayerInput.gameObject.SetActive(false);
        Inputbutton.gameObject.SetActive(false);
        nextbutton.gameObject.SetActive(false);


        speaker.text = line.speaker;  //Name
        NPCscript.text = line.text;

        //UI 표시 처리
        if (line.speaker == "NPC")   // NPC가 말할 경우
        {
            // NPC 대사 - 다음 버튼 표시
            NPCscript.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(true);
        }
        else if (line.speaker == "???")   //플레이어가 말할 경우
        {
            PlayerInput.gameObject.SetActive(true);
            Inputbutton.gameObject.SetActive(true);
        }
    }
    void ShowNextScript()
    {
        dialogueIndex++;

        if (dialogueIndex < currentDialogue.dialoglines.Length)
        {
            ShowCurrentScript();
        }
        else
        {
            // 대화 끝 처리 - 필요에 따라 UI 비우기 -> 추후에 엔딩이나 다음 일차로 넘어가도록 수정
            NPCscript.text = "";
            speaker.text = "";
            nextbutton.gameObject.SetActive(false);

            // 여기서 AINPCscene.AdvanceDay()를 호출해서 배경을 갱신  -> 0516수정
            // 다음 날로 이동
            if (sceneController != null)
                sceneController.AdvanceDay();
            else
                Debug.LogWarning("AINPCscene 인스턴스를 찾을 수 없습니다.");
        }
    }
}
