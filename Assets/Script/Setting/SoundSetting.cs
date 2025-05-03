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

     void Start()
    {
        // 초기 설정
        ApplySettings();
    }


    public void PlayBgm(int index)
    {
        if (sounddata.isBgmOn)
        {
            bgmSource.clip = sounddata.bgmClips[index];
            bgmSource.Play();
        }
    }

    //SoundManager.Instance.PlayBGM(index); 로 호출 -> SoundData에셋 활용

    // SFX 재생
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }


    //SoundControl.Instance.PlaySfx(index); 로 호출 -> SoundData에셋 활용

    public void ApplySettings()  //초기 설정
    {
        float bgmLinear = Mathf.Clamp(sounddata.bgmvolume, 0.0001f, 1f);
        float sfxLinear = Mathf.Clamp(sounddata.sfxvolume, 0.0001f, 1f);  
        // 0일 경우 안전값

        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(bgmLinear) * 20f : -80f;
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sfxLinear) * 20f : -80f;

      

        audioMixer.SetFloat("Bgmvolume", bgmDb);   // AudioMixer 초기값 설정
        audioMixer.SetFloat("Sfxvolume", sfxDb);   // AudioMixer 초기값 설정
    }



}//  SoundControl.SetFloat("sfxvolume", Mathf.Log10(Slidervalue) * 20);