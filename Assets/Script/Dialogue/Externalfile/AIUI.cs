using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;  // Regex 클래스
using System.Linq;
using System;   // Skip, Take

public class AIUI : MonoBehaviour
{
    [Header("대사 출력 효과")] 
    public float charDelay = 0.05f;  // 글자 출력 딜레이
    public AudioClip charSFX;

    [Header("UI")]
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

    public DayCheck dayCheck;

    [Header("DayCheck Reference")]
    public DayCheck daycheck;

    [Header("Advance Day Settings")]
    [Tooltip("수노와는 총 5번 대화할 수 있다")]
    public int responsesToAdvance = 5;
    private int sunoResponseCount = 0;



    private void Start()
    {

        SoundSetting.Instance.PlayBgm(3);  //3번 BGM
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
            {
                sunoResponseCount++;
                if (sunoResponseCount >= responsesToAdvance)
                {
                    StartCoroutine(daycheck.AdvanceDayAndSave(daycheck.CurrentDay + 1));  // 다음요일로 넘어가기
                    yield break;
                }
                SUNOImage.color = Color.white;
            }
           
        }         
          
            replynpcscript.text = reply.line;

            sentences = Regex.Split(reply.line, @"(?<=[\.!\?\,])\s+");  //. / ! / ? / , 뒤의 공백을 기준으로 분리
            currentIndex = 0;

            replynpcscript.gameObject.SetActive(true);
            nextbutton.gameObject.SetActive(true);

            ShowNextScript();
        }


    public void ShowNextScript()
    {
        SoundSetting.Instance.PlaySfx(9);  // sfx9

        if (currentIndex < sentences.Length)  // 아직 마지막 대사까지 보여주지 못한 경우
        {
            int take = Mathf.Min(2, sentences.Length - currentIndex);  // 대사 길이에-서 현재 인덱스 제외 (2문장씩 나누기)

            string divided = string.Join(" ", sentences   // 첫번째 인자 " "인 구분자를 사이사이에 넣고 하나의 긴 string으로 합침
                .Skip(currentIndex)
                //현재까지 보여준 문장 수(currentIndex)만큼 처음 요소를
                //건너뛰고 그 뒤의 요소들만 남김
                .Take(take));
            //건너뛴 뒤 남은 요소 중에서
            //최대 take 개수(여기서는 두 문장)를 가져옴
            currentIndex += take;

            nextbutton.interactable = false;     // 타이핑 중에는 누를 수 없게
            StopAllCoroutines();                 // 이전 코루틴 정리

            StartCoroutine(TypeEffect(divided, () =>
            //divided 문자열을 한 글자씩 찍어낸 뒤,
            // () => : 찍기가 끝난 시점에 { … } 블록 안의 로직을 동작 -> ChatGPT 참고
            {
                // 한 덩어리 타이핑이 끝난 뒤 Next 버튼 활성화
                nextbutton.interactable = true;
                if (currentIndex >= sentences.Length)
                    lastScriptShown = true;
            }));

            return;
        }
        if (lastScriptShown)// 이미 마지막대사까지 보여준 상태
        {
            //Npc 스크립트 UI 비활성화
            replynpcscript.gameObject.SetActive(false);
            nextbutton.gameObject.SetActive(false);

            speaker.text = "나";

            SUNOImage.color = new Color(170f / 255f, 170f / 255f, 170f / 255f);   //내가 이야기 할 때 수노는 회색으로 

            //Player 스크립트 UI 활성화
            PlayerInput.gameObject.SetActive(true);
            Inputbutton.gameObject.SetActive(true);
            lastScriptShown = false;
        }

    }


    IEnumerator TypeEffect(string sentence, Action onComplete = null)     //타이핑 효과 코루틴
        // Action 활용 System 네임스페이스에 정의된 파라미터 없고 반환값도 없는(delegate void) 대표 델리게이트
        { replynpcscript.text = "";
            foreach (char c in sentence)
            {
                replynpcscript.text += c;
                if (charSFX != null)
                {
                    SoundSetting.Instance.PlaySfx(7);  //7번 효과음 재생
                }
                yield return new WaitForSeconds(charDelay);  // 0.05f 만큼 딜레이되며 출력
            }
            onComplete?.Invoke();
            // onComplete가 null이 아닌지 검사 ->onComplete? : null이 아니면 이어지는 호출을 수행
            // .Invoke(); Action 델리게이트(메서드 포인터)를 실제로 호출하는 메서드
        }


    void SomeMethodThatAdvancesDay(int nextDay)
    {
        // ① 초기화 플래그 검사
        if (!dayCheck.IsInitialized)
        {
            Debug.LogError("세이브 정보가 아직 준비되지 않았습니다! AdvanceDayAndSave 호출을 취소합니다.");
            return;
        }

        // ② 준비가 끝났으면 안전하게 코루틴 실행
        StartCoroutine(dayCheck.AdvanceDayAndSave(nextDay));
    }

}
