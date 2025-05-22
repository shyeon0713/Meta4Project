using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class AIUI : MonoBehaviour
{
    public TMP_Text speaker;
    public TMP_Text replynpcscript;
    public TMP_InputField PlayerInput;
    public Button Inputbutton;
    public Button nextbutton;

    public DialogueAPI dialogueapi;

    [Header("DayCheck")]
    public DayCheck daycheck;

    [Header("AINPC_Back")]
    public DayCheck Background;

    private void Start()
    {
        Inputbutton.onClick.AddListener(InputSend);
        nextbutton.onClick.AddListener(PlayerSetvisible);

        // Player의 입력으로 먼저 시작
        replynpcscript.gameObject.SetActive(false);
        nextbutton.gameObject.SetActive(false);

    }

    void InputSend()
    {
        string message = PlayerInput.text;
        string currentSpeaker = speaker.text;
        PlayerInput.text = "";

        PlayerInput.gameObject.SetActive(false);
        Inputbutton.gameObject.SetActive(false);

        StartCoroutine(GetAndShowReply(message, currentSpeaker));

        speaker.text = " ";  // UI에 남은 텍스트 초기화
    }

    IEnumerator GetAndShowReply(string message, string speakerName)
    {
        yield return StartCoroutine(dialogueapi.SendPlayerReply(message, speakerName));

        DialogueLine reply = dialogueapi.savescript;

        if (reply != null)
        {
            speaker.text = reply.speaker;
            replynpcscript.text = reply.line;

            replynpcscript.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(true);
        }
        else
        {
            replynpcscript.text = "생성응답 없음";
            replynpcscript.gameObject.SetActive(true);
        }
    }

    void PlayerSetvisible()  // next 버튼을 누를 경우, Player의 답변 활성화 및 NPC스크립트는 초기화
    {
        //NPC 스크립트 비활성화
        replynpcscript.text = " ";  //이전 대사 삭제
        replynpcscript.gameObject.SetActive(false);
        nextbutton.gameObject.SetActive(false);
        speaker.text = " ";  //speaker 비우기

        //Player 스크립트 비활성화
        PlayerInput.gameObject.SetActive(true);
        Inputbutton.gameObject.SetActive(true);
    }

}
