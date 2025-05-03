using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIUI : MonoBehaviour
{
    public TMP_Text speaker;

    // NPC 부분
    public TMP_Text NPCscript;
    public Button nextbutton; // 스크립트 넘김 버튼

    //플레이어 답변입력 부분
    public TMP_InputField PlayerInput;
    public Button Inputbutton;  //입력 버튼
  
    public DialogueAPI dialogueService;

    private extraDialogueData currentDialogue;
    private int dialogueIndex = 0;

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
        if (currentDialogue.dialoglines.Length > 0)
        {
            ShowCurrentScript();  
        }
    }
    void ShowCurrentScript()  //현재 화면에 표시되는 UI 설정
    {
        DialogueLine line = currentDialogue.dialoglines[dialogueIndex];

        speaker.text = line.speaker;
        NPCscript.text = line.text;

        //UI 표시 처리
        if (line.speaker == "NPC")   // NPC가 말할 경우
        {
            // NPC 대사 - 다음 버튼 표시
            PlayerInput.gameObject.SetActive(false);
            Inputbutton.gameObject.SetActive(false);
            nextbutton.gameObject.SetActive(true);
        }
        else if (line.speaker == "???")   //플레이어가 말할 경우
        {
            PlayerInput.gameObject.SetActive(true);
            Inputbutton.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(false);
        }
        else // 기본처리  -> NPC가 말하는 상태 : 필요없을 경우 제외
        {
            PlayerInput.gameObject.SetActive(false);
            Inputbutton.gameObject.SetActive(false);
            nextbutton.gameObject.SetActive(true);
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
        }
    }
}
