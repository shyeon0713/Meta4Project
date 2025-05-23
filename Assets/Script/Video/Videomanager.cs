using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class Videomanager : MonoBehaviour
{
    public static Videomanager Instance { get; private set; }

    [Header("videoclips")]
    public VideoClip[] videoclips;

///    [Tooltip("영상이 끝난뒤 넘어가야하는 씬")]


    [HideInInspector]   //선언된 필드를 인스펙터 창에서 숨길때 사용
    public int selectedIndex;   //영상 리스트 인덱스 

    public string videoname; // 영상 이름
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //중복 방지 -> 이미선택된 오브젝트가 있을 경우 선택X
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadVideo(int index)
    {
        if(index < 0 || index >= videoclips.Length)
        {
            Debug.LogError("Videomanager : index {index}");
            return;
        }
        selectedIndex = index;
        SceneManager.LoadScene("videoscene");  // 외부에서 index설정 후, videoscene로 씬전환 
    }

    // 씬 호출 시 사용 :  Videomanager.Instance.LoadVideo(index);  index = int
}
