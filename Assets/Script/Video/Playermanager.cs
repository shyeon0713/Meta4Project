using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
public class Playermanager : MonoBehaviour
{
    [Header("비디오 재생 후 Activescene로만 넘어감")]
    public string nextSceneName = "Activescene";

    [Header("Videomanager에서 관리하는 클립 인덱스")]
    private int clipIndex;

    [Header("각 클립에 대응하는 Day 값")]
    [Tooltip("videoclips.Length와 동일한 길이여야 합니다.")]
    public int[] dayValues;

    private VideoPlayer vp;

    float likeability;
    int lastdialogueid;
    string lastspeaker;
    string lastline;


    public Button skipButton;  //스킵버튼 추가
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
        Debug.Log("[Playermanager] Start() – clipIndex=" + clipIndex);
        clipIndex = Videomanager.Instance.selectedIndex;
        var clips = Videomanager.Instance.videoclips;


        // 유효성 검사
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


        vp.clip = clips[clipIndex];
        vp.Play();
    }


    void VideoFinished(VideoPlayer source)   // 영상이 끝난 후 
    {
        // if (string.IsNullOrEmpty(nextSceneName[))
        // {
        //      Debug.LogError("씬이 없음");
        //     return;
        //  }
        Debug.Log("[Playermanager] VideoFinished() fired");
        StartCoroutine(UpdateDayAndLoadScene(dayValues[clipIndex]));
    }

    private void SkipVideo()   //Skip 버튼 리스너
    {
        Debug.Log("[Playermanager] SkipVideo() called");
        SoundSetting.Instance.PlaySfx(4);  //효과음
        vp.loopPointReached -= VideoFinished;
        if (vp.isPlaying) vp.Stop();
        StartCoroutine(UpdateDayAndLoadScene(dayValues[clipIndex]));
    }

    private IEnumerator UpdateDayAndLoadScene(int day)
    {
        Debug.Log($"▶[Playermanager] 진입: clipIndex={clipIndex}, day={day}");
        string url = "http://127.0.0.1:8000/save/";
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
                last_dialogue_id = 1,    // 대사 ID는 0 (서버에서 NULL로 매핑하도록)
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



//StartCoroutine(dayCheck.AdvanceDayAndSave(nextDay)); -> Day2 이상의 컷씬