using System;
using System.Collections;
//using System.Net.Sockets;
using System.Text;
using UnityEditor.PackageManager.Requests;

//using Unity.VisualScripting;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Playermanager : MonoBehaviour
{
    private const string API_URL = "http://127.0.0.1:8000/dialogue/start";  //startapi주소
   // private const string url = "http://127.0.0.1:8000/save/";

    [Header("비디오 재생 후 Activescene로만 넘어감")]
    public string nextSceneName = "Activescene";

    [Header("Videomanager에서 관리하는 클립 인덱스")]
    private int clipIndex;

    [Header("각 클립에 대응하는 Day 값")]
    [Tooltip("videoclips.Length와 동일한 길이여야 합니다.")]
    public int[] dayValues;

    private VideoPlayer vp;

    [Header("Save 변수들")]
    float likeability;  // 호감도
    int lastdialogueid;  // 마지막 대사 인덱스
    string lastspeaker;  // 마지막 발화자
    string lastline;  // 마지막 대사
    int slotnumber; // 세이브 슬롯 넘버

    [Header("버튼")]
    public Button skipButton;  //스킵버튼 추가
                               // Startapi 추가 
                               // Startapi -> 스킵버튼 + 영상재생이 종료된 후

   
    private bool isCallingApi = false;  // API 호출 중복 방지

    private void Awake()
    {
       // Debug.Log("[Playermanager] Awake()");
        vp = GetComponent<VideoPlayer>();

        if(vp == null)
        {
            Debug.LogError("비디오가 없어요");
            enabled = false;
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
        //현재 재생되는 영상 인덱스
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

        skipButton.onClick.RemoveAllListeners(); // 기존의 리스너 제거

        skipButton.onClick.AddListener(SkipVideo);   //SKip 버튼 리스너


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

        // 영상 종료 확인
        Debug.Log("[Playermanager] VideoFinished() fired");

        if (!isCallingApi)   // 영상이 끝났을 경우
        {
            //StartAPI 호출
            StartCoroutine(StartapiConnect());
        }
    }
    #endregion

    #region   스킵버튼을 눌렀을 경우 -> 영상종료 후, 다음씬 이동 + StartAPI 호출
    private void SkipVideo()   //Skip 버튼 리스너
    {
        Debug.Log("[Playermanager] SkipVideo() called");
        SoundSetting.Instance.PlaySfx(4);  //효과음

        if (vp != null) {

            vp.loopPointReached -= VideoFinished;  //비디오 이벤트 제거
            if (vp.isPlaying) vp.Stop();   //비디오 정지
        }


        if (!isCallingApi)   // 영상이 끝났을 경우
        {
            //StartAPI 호출 (중복 방지)
            StartCoroutine(StartapiConnect());
        }
    }
    #endregion


    private IEnumerator StartapiConnect()
    {
        // 중복 호출을 방지하기 위해 플래그 설정
        isCallingApi = true;

        // Start API 호출
        string startapi = "string";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(startapi);

        // 요청 생성만 하고 아직 body, 헤더가 지정되지 않음
        using (UnityWebRequest request = new UnityWebRequest(API_URL, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene(nextSceneName);   // StartAPI 연동 후, 다음 씬으로 이동
            
            }
            else   // startapi연결이 제대로 진행되지 않은 경우
            {
                Debug.LogError($"[Playermanager] API ❌ 오류 : {request.error} | Code: {request.responseCode}");
                Debug.LogError("[Playermanager] Response Body: " + request.downloadHandler.text);
            }
        }

        isCallingApi=false;
    }

 
}

//StartCoroutine(dayCheck.AdvanceDayAndSave(nextDay)); -> Day2 이상의 컷씬