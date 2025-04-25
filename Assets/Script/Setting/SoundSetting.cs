using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundSetting : MonoBehaviour
{
    public static SoundSetting Instance { get; private set; }  // 싱글톤 인스턴스 선언

    public AudioMixer audioMixer;
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public SoundData sounddata;  //ScriptableObject 참조

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  //현재 생성된 오브젝트를 전역에서 접근 가능한 인스턴스로 지정
            DontDestroyOnLoad(gameObject);  //씬이 전환되어도 오브젝트를 유지
        }
        else
        {
            Destroy(gameObject);  
            return;
        }
    }

    private void Start()
    {
        // 초기 설정
        ApplySettings();
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때 호출될 메서드
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)  //씬이 로드될 때 호출될 메서드
    {
        SetBgmForScene(scene.name); // 씬 이름에 맞는 BGM을 설정
    }



    // 씬 이름에 맞는 BGM을 설정
    public void SetBgmForScene(string sceneName)
    {
        int sceneIndex = GetSceneBgmIndex(sceneName); // 씬에 맞는 BGM 인덱스를 반환
        PlayBgm(sceneIndex); // 해당 인덱스로 BGM 재생
    }


    // 씬 이름에 맞는 BGM 인덱스를 반환
    private int GetSceneBgmIndex(string sceneName)
    {
        // 예시로, 씬 이름에 따라 인덱스를 다르게 설정
        if (sceneName == "MainMenu") return 0; 
        if (sceneName == "Level1") return 1; 
        if (sceneName == "Level2") return 2;
        if (sceneName == "GameOver") return 3; 
        return 0; // 기본 BGM
    }

    public void PlayBgm(int index)
    {
        bgmSource.clip = sounddata.bgmClips[index];
        bgmSource.Play();
    }

    // SFX 재생
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }


    //SoundManager.Instance.PlaySfx(indexvalue); 로 호출



    public void ApplySettings()  //초기 설정
    {
        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(sounddata.bgmvolume) * 20f : -80f;  // Slider 초기 설정 , AudioMixer 설정활용
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sounddata.sfxvolume) * 20f : -80f;  // Slider 초기 설정 , AudioMixer 설정활용

        audioMixer.SetFloat("BgmVolume", bgmDb);   // AudioMixer 초기값 설정
        audioMixer.SetFloat("SfxVolume", sfxDb);   // AudioMixer 초기값 설정
    }



}
