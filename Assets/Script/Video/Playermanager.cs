using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Playermanager : MonoBehaviour
{
    [Header("���� ��� �� Activescene�θ� �Ѿ")]
    public string nextSceneName = "Activescene";

    private VideoPlayer vp;

    public Button skipButton;  //��ŵ��ư �߰�
    private void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        if(vp == null)
        {
            Debug.LogError("������ �����");
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
            Debug.LogError("����� Ȯ����");
            return;
        }

        int index = manager.selectedIndex;
        vp.clip = manager.videoclips[index];
        vp.Play();
    }


    void VideoFinished(VideoPlayer source)   // ������ ���� �� 
    {
       // if (string.IsNullOrEmpty(nextSceneName[))
      // {
      //      Debug.LogError("���� ����");
       //     return;
      //  }

        SceneManager.LoadScene(nextSceneName);   // ���̵�
    }


    void OnDestroy()
    {
        if (vp != null)
            vp.loopPointReached -= VideoFinished;
    }

    void SkipVideo()  // ���� ��ŵ�� ���
    {
        vp.loopPointReached -= VideoFinished;
        if (vp.isPlaying)
        {
            vp.Stop();
        }
        // �� ��ȯ
        SceneManager.LoadScene(nextSceneName);
    }

}
