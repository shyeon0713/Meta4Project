using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;  // Regex 클래스
using System.Linq;   // Skip, Take

public class AIUI : MonoBehaviour
{
    public TMP_Text speaker;
    public TMP_Text replynpcscript;
    public TMP_InputField PlayerInput;
    public Button Inputbutton;
    public Button nextbutton;

    public Image SUNOImage;  // 수노 스프라이트

    private string[] sentences; //문장별 분할 결과
    private int currentIndex = 0; //현재 읽어야 할 문장 인덱스

    private bool lastScriptShown = false;  // 마지막 대사까지 출력되는지 여부 확인
    // 2문장씩 나눠서 출력 시키기 -> 0522


    public DialogueAPI dialogueapi;

    [Header("DayCheck")]
    public DayCheck daycheck;

    [Header("AINPC_Back")]
    public DayCheck Background;

    private void Start()
    {
        speaker.text = "나";
        SUNOImage.color = new Color(170f / 255f, 170f / 255f, 170f / 255f); //수노는 회색
        Inputbutton.onClick.AddListener(InputSend);
        nextbutton.onClick.AddListener(ShowNextScript);

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

   //     Debug.Log("▶ GetAndShowReply 시작");
   //     if (dialogueapi == null) Debug.LogError("dialogueapi가 null입니다!");

        yield return StartCoroutine(dialogueapi.SendPlayerReply(message, speakerName));

        DialogueLine reply = dialogueapi.savescript;
        speaker.text = reply.speaker;

     /*   if (reply == null)
        {
            Debug.LogError("reply가 null입니다!");
            yield break;
        }

        Debug.Log($"reply.speaker: {reply.speaker}, reply.line: {reply.line}");

        if (speaker == null) Debug.LogError("speaker(Text) 참조가 없습니다!");
        if (replynpcscript == null) Debug.LogError("replynpcscript(Text) 참조가 없습니다!");
        if (nextbutton == null) Debug.LogError("nextbutton(Button) 참조가 없습니다!");

     */
        if (reply != null)
        {
            if (reply.speaker == "수노")
            { SUNOImage.color = Color.white; }           
          
            replynpcscript.text = reply.line;

            sentences = Regex.Split(reply.line, @"(?<=[\.!\?\,])\s+");  //. / ! / ? / , 뒤의 공백을 기준으로 분리
            currentIndex = 0;

            replynpcscript.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(true);

            ShowNextScript();
        }

      /*  else
        {
            replynpcscript.text = "생성응답 없음";
            replynpcscript.gameObject.SetActive(true);
        }

        */
        
    }

    public void ShowNextScript()
    {
        if (currentIndex < sentences.Length)  // 아직 마지막 대사까지 보여주지 못한 경우
        {

            int remainscript = sentences.Length - currentIndex;
            if (remainscript <= 0) return;

            int take = Mathf.Min(2, remainscript);  // 두 문장씩 나누기

            string divided = string.Join(" ", sentences   // 첫번째 인자 " "인 구분자를 사이사이에 넣고 하나의 긴 string으로 합침
                .Skip(currentIndex)
                //현재까지 보여준 문장 수(currentIndex)만큼 처음 요소를
                //건너뛰고 그 뒤의 요소들만 남김
                .Take(take));
            //건너뛴 뒤 남은 요소 중에서
            //최대 take 개수(여기서는 두 문장)를 가져옴
            replynpcscript.text = divided;

            currentIndex += take;

            if (currentIndex >= sentences.Length)
            {  // next 버튼을 누를 경우, Player의 답변 활성화 및 NPC스크립트는 초기화
                lastScriptShown = true;

            }
            return;
        }


        if(lastScriptShown)// 이미 마지막대사까지 보여준 상태
        { 
            replynpcscript.gameObject.SetActive(false);
            nextbutton.gameObject.SetActive(false);

            speaker.text = "나";

            SUNOImage.color = new Color(170f / 255f, 170f / 255f, 170f / 255f);   //내가 이야기 할 때 수노는 회색으로 

            //Player 스크립트 활성화
            PlayerInput.gameObject.SetActive(true);
            Inputbutton.gameObject.SetActive(true);

        }


    }

}
