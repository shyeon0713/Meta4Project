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
    }


    public void PlayBGM(int index)
    {
        if (sounddata.isSfxOn)
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }

    //SoundManager.Instance.Play BGM(index); 로 호출 -> SoundData에셋 활용

    // SFX 재생
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }


    //SoundManager.Instance.PlaySfx(index); 로 호출 -> SoundData에셋 활용

    public void ApplySettings()  //초기 설정
    {
        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(sounddata.bgmvolume) * 20f : -80f;  // Slider 초기 설정 , AudioMixer 설정활용
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sounddata.sfxvolume) * 20f : -80f;  // Slider 초기 설정 , AudioMixer 설정활용

        audioMixer.SetFloat("BgmVolume", bgmDb);   // AudioMixer 초기값 설정
        audioMixer.SetFloat("SfxVolume", sfxDb);   // AudioMixer 초기값 설정
    }



}
