using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Videomanager : MonoBehaviour
{
    public static Videomanager Instance { get; private set; }

    [Header("videoclips")]
    public VideoClip[] videoclips;

    [HideInInspector]   //선언된 필드를 인스펙터 창에서 숨길때 사용
    public int selectedIndex;   //리스트 인덱스 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
