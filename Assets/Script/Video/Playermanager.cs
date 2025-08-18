using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
public class Playermanager : MonoBehaviour
{
    private const string API_URL = "http://127.0.0.1:8000/dialogue/start";  //startapi주소
    private const string url = "http://127.0.0.1:8000/save/";

    [Header("비디오 재생 후 Activescene로만 넘어감")]
    public string nextSceneName = "Activescene";

    [Header("Videomanager에서 관리하는 클립 인덱스")]
    private int clipIndex;

    [Header("각 클립에 대응하는 Day 값")]
    [Tooltip("videoclips.Length와 동일한 길이여야 합니다.")]
    public int[] dayValues;

    private VideoPlayer vp;

    [Header("Save 변수들")]
    float likeability;
    int lastdialogueid;
    string lastspeaker;
    string lastline;

    [Header("버튼")]
    public Button skipButton;  //스킵버튼 추가
    // Startapi 추가 
    // Startapi -> 스킵버튼 + 영상재생이 종료된 후

    private void Awake()
    {
        Debug.Log("[Playermanager] Awake()");
        vp = GetComponent<VideoPlayer>();
        if(vp == null)
        {
            Debug.LogError("비디오가 없어요");
            return;
        }

        vp.loopPointReached += VideoFinished;
        //VideoPlayer.loopPointReached

    }

    void Start()
    {
        SoundSetting.Instance.bgmSource.Stop();
        // 모든 브금 완전 정지 -> 현재 SoundManager내에 일시정지, 완전 정지 메서드를 따로 구현하지 않아 직접 호출
        // 일시정지 : SoundSetting.Instance.bgmSource.Pause();
        Debug.Log("[Playermanager] Start() – clipIndex=" + clipIndex);
        clipIndex = Videomanager.Instance.selectedIndex;
        var clips = Videomanager.Instance.videoclips;


        // 유효성 검사 -> 추후에 주석처리 하기
        if (clips == null || clips.Length == 0)
        {
            Debug.LogError("Videomanager에 클립이 없습니다!");
            return;
        }
        if (dayValues == null || clipIndex < 0 || clipIndex >= dayValues.Length)
        {
            Debug.LogError("dayValues 배열의 길이가 videoclips와 일치하지 않습니다!");
            return;
        }
        if (clipIndex < 0 || clipIndex >= clips.Length)
        {
            Debug.LogError($"잘못된 selectedIndex: {clipIndex}");
            return;
        }


        skipButton.onClick.AddListener(SkipVideo);   //SKip 버튼 리스너
        skipButton.onClick.AddListener(CallStartAPI);  //API를 호출하는 코루틴 메서드


        vp.clip = clips[clipIndex];
        vp.Play();
    }


    #region 영상을 끝까지 시청할 경우 -> 영상종료 후, 다음씬 이동 + StartAPI호출
    void VideoFinished(VideoPlayer source)   // 영상이 끝난 후 
    {
        // if (string.IsNullOrEmpty(nextSceneName[))
        // {
        //      Debug.LogError("씬이 없음");
        //     return;
        //  }

        // 영상 종료
        Debug.Log("[Playermanager] VideoFinished() fired");
        StartCoroutine(UpdateDayAndLoadScene(dayValues[clipIndex]));


        //StartAPI 호출
        StartCoroutine(StartapiConnect());
    }
    #endregion

    #region   스킵버튼을 눌렀을 경우 -> 영상종료 후, 다음씬 이동 + StartAPI 호출
    private void SkipVideo()   //Skip 버튼 리스너
    {
        Debug.Log("[Playermanager] SkipVideo() called");
        SoundSetting.Instance.PlaySfx(4);  //효과음

        vp.loopPointReached -= VideoFinished;  //비디오 이벤트 제거
        if (vp.isPlaying) vp.Stop();   //비디오 정지

        StartCoroutine(UpdateDayAndLoadScene(dayValues[clipIndex]));

    }


    private void CallStartAPI()   //StartAPI 호출 용도
    {
        StartCoroutine(StartapiConnect());
    }



    private IEnumerator StartapiConnect()
    {
        // Start API 호출
        string startapi = "string";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(startapi);

        // 요청 생성만 하고 아직 body, 헤더가 지정되지 않음
        UnityWebRequest request = new UnityWebRequest(API_URL, UnityWebRequest.kHttpVerbPOST);

        if (request.result == UnityWebRequest.Result.Success)
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");

            yield return request.SendWebRequest();
        }
        else   // startapi연결이 제대로 진행되지 않은 경우
        {
            Debug.LogError($"API Error: {request.error} | Code: {request.responseCode}");
            Debug.LogError($"Response Body: {request.downloadHandler.text}");
        }


    }

    #endregion


    #region 다음씬 넘어가기전에 초기설정값 전송
    private IEnumerator UpdateDayAndLoadScene(int day)
    {
        Debug.Log($"▶[Playermanager] 진입: clipIndex={clipIndex}, day={day}");
       
        Debug.Log($"▶ URL: {url}");

        // Intro 영상(인덱스 0)에 해당하는 초기 세팅
        SaveFile savefile;
        if (day == 0)
        {
            // Intro 영상인 경우에는 Day1 초기값을 강제 세팅
            savefile = new SaveFile
            {
                day = 1,    // 기본 Day 1
                likeability = 3.0f,    // 초기 호감도
                last_dialogue_id = 0,    // 대사 ID는 0 (서버에서 NULL로 매핑하도록)
                last_speaker = "",   // 빈 문자열
                last_line = ""
            };
        }
        else
        {
            // 일반 영상(인덱스 1,2,3…)인 경우에는 day 매개변수값을 그대로 사용
            savefile = new SaveFile
            {
                day = day,
                likeability = likeability,
                last_dialogue_id = lastdialogueid,
                last_speaker = lastspeaker,
                last_line = lastline
            };
        }

        // json 직렬화 -> 이것때문에 서버에 마지막 대사,대사인덱스,발화자를 꼭 입력해줘야함
        string json = JsonUtility.ToJson(savefile);
        Debug.Log("▶ 보낼 JSON: " + json);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        // Post 요청 생성
        var req = new UnityWebRequest(url, "POST");  //POST로 요청
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");


        // 요청 전송
        yield return req.SendWebRequest();


        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Day 업데이트 실패: HTTP {req.responseCode} | {req.error}");
            Debug.LogError($"서버 응답 본문: {req.downloadHandler.text}");
        }
        else
        {
            // 성공했으면 씬 전환
            SceneManager.LoadScene(nextSceneName);
        }
    }
    private void OnDestroy()
    {
        if (vp != null)
            vp.loopPointReached -= VideoFinished;
    }
}

#endregion

//StartCoroutine(dayCheck.AdvanceDayAndSave(nextDay)); -> Day2 이상의 컷씬