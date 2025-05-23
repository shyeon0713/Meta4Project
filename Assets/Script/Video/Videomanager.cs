using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class Videomanager : MonoBehaviour
{
    public static Videomanager Instance { get; private set; }

    [Header("videoclips")]
    public VideoClip[] videoclips;

///    [Tooltip("������ ������ �Ѿ���ϴ� ��")]


    [HideInInspector]   //����� �ʵ带 �ν����� â���� ���涧 ���
    public int selectedIndex;   //���� ����Ʈ �ε��� 

    public string videoname; // ���� �̸�
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //�ߺ� ���� -> �̹̼��õ� ������Ʈ�� ���� ��� ����X
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
        SceneManager.LoadScene("videoscene");  // �ܺο��� index���� ��, videoscene�� ����ȯ 
    }

    // �� ȣ�� �� ��� :  Videomanager.Instance.LoadVideo(index);  index = int
}
