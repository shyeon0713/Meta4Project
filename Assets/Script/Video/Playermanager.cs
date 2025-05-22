using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Playermanager : MonoBehaviour
{
    [Header("비디오 재생 후 Activescene로만 넘어감")]
    public string nextSceneName = "Activescene";

    private VideoPlayer vp;

    public Button skipButton;  //스킵버튼 추가
    private void Awake()
    {
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

        skipButton.onClick.AddListener(SkipVideo);

        var manager = Videomanager.Instance;
        if(manager == null)
        {
            Debug.LogError("비디어설정 확인좀");
            return;
        }

        int index = manager.selectedIndex;
        vp.clip = manager.videoclips[index];
        vp.Play();
    }


    void VideoFinished(VideoPlayer source)   // 영상이 끝난 후 
    {
       // if (string.IsNullOrEmpty(nextSceneName[))
      // {
      //      Debug.LogError("씬이 없음");
       //     return;
      //  }

        SceneManager.LoadScene(nextSceneName);   // 씬이동
    }


    void OnDestroy()
    {
        if (vp != null)
            vp.loopPointReached -= VideoFinished;
    }

    void SkipVideo()  // 영상 스킵인 경우
    {
        vp.loopPointReached -= VideoFinished;
        if (vp.isPlaying)
        {
            vp.Stop();
        }
        // 씬 전환
        SceneManager.LoadScene(nextSceneName);
    }

}
