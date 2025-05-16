using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIUI : MonoBehaviour
{
    public TMP_Text speaker;// Name ǥ��

    // NPC �κ�
    public TMP_Text NPCscript;
    public Button nextbutton; // ��ũ��Ʈ �ѱ� ��ư

    //�÷��̾� �亯�Է� �κ�
    public TMP_InputField PlayerInput;  //���� �Է�
    public Button Inputbutton;  //�Է� ��ư
  
    public DialogueAPI dialogueService;

    private extraDialogueData currentDialogue;
    private int dialogueIndex = 0;

    [Header("Scene Controller")]
    public DayCheck sceneController;

    private void Start()
    {
        Inputbutton.onClick.AddListener(InputSend);
        nextbutton.onClick.AddListener(ShowNextScript);

        //�÷��̾� �亯�κ� �����
        Inputbutton.gameObject.SetActive(false);
        PlayerInput.gameObject.SetActive(false);

    }

    void InputSend()
    {
        string playerInput = PlayerInput.text;
        PlayerInput.text = "";

        //�Է� �� �ٽ� ������ ����
        StartCoroutine(dialogueService.SendPlayerReply(playerInput, OnDialogueReceived));
    }

    void OnDialogueReceived(string json)  
    {
        currentDialogue = extraDialogueData.extraFromJson(json);  //extraDialogueData���� ����
        dialogueIndex = 0; //���Ƿ� �ε����� int�� ǥ�� 
        if (currentDialogue.dialoglines != null && currentDialogue.dialoglines.Length > 0)
        {
            ShowCurrentScript();  
        }
    }
    void ShowCurrentScript()  //���� ȭ�鿡 ǥ�õǴ� UI ����
    {
        DialogueLine line = currentDialogue.dialoglines[dialogueIndex];

        // �⺻������ ��� UI ���� -> ���ǹ����� UI �������� ����
        speaker.gameObject.SetActive(false);
        NPCscript.gameObject.SetActive(false);
        PlayerInput.gameObject.SetActive(false);
        Inputbutton.gameObject.SetActive(false);
        nextbutton.gameObject.SetActive(false);


        speaker.text = line.speaker;  //Name
        NPCscript.text = line.text;

        //UI ǥ�� ó��
        if (line.speaker == "NPC")   // NPC�� ���� ���
        {
            // NPC ��� - ���� ��ư ǥ��
            NPCscript.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(true);
        }
        else if (line.speaker == "???")   //�÷��̾ ���� ���
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
            // ��ȭ �� ó�� - �ʿ信 ���� UI ���� -> ���Ŀ� �����̳� ���� ������ �Ѿ���� ����
            NPCscript.text = "";
            speaker.text = "";
            nextbutton.gameObject.SetActive(false);

            // ���⼭ AINPCscene.AdvanceDay()�� ȣ���ؼ� ����� ����  -> 0516����
            // ���� ���� �̵�
            if (sceneController != null)
                sceneController.AdvanceDay();
            else
                Debug.LogWarning("AINPCscene �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }
}
