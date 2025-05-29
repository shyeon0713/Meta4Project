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

    // PlayerPrefs 키 
    const string KEY_BGM_VOL = "BgmVolume";
    const string KEY_SFX_VOL = "SfxVolume";
    const string KEY_BGM_ON = "BgmOn";
    const string KEY_SFX_ON = "SfxOn";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 할당 (인스펙터에 안 넣었다면)
            if (bgmSource == null) bgmSource = GetComponent<AudioSource>();

            LoadSettings();
            ApplySettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #region 설정 Load/Save  
    // 코드 폴더처럼 에디터 상에서 관련 메서드를 묶어주기 위한 C#의 편의 기능
    void LoadSettings()
    {
        sounddata.bgmvolume = PlayerPrefs.GetFloat(KEY_BGM_VOL, 1f);
        sounddata.sfxvolume = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);
        sounddata.isBgmOn = PlayerPrefs.GetInt(KEY_BGM_ON, 1) == 1;
        sounddata.isSfxOn = PlayerPrefs.GetInt(KEY_SFX_ON, 1) == 1;
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat(KEY_BGM_VOL, sounddata.bgmvolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sounddata.sfxvolume);
        PlayerPrefs.SetInt(KEY_BGM_ON, sounddata.isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SFX_ON, sounddata.isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion


    #region  외부 호출 API -> Playerprefs API 호출
    public void SetBgmVolume(float v)
    {
        sounddata.bgmvolume = v;
        ApplySettings();
        SaveSettings();
    }

    public void SetSfxVolume(float v)
    {
        sounddata.sfxvolume = v;
        ApplySettings();
        SaveSettings();
    }

    public void BgmOnOff()
    {
        sounddata.isBgmOn = !sounddata.isBgmOn;
        ApplySettings();
        SaveSettings();
    }

    public void SfxOnOff()
    {
        sounddata.isSfxOn = !sounddata.isSfxOn;
        ApplySettings();
        SaveSettings();
    }
    #endregion


    #region 내부 적용 로직
    public void ApplySettings()   //초기 셋팅
    {
        float bgmLin = Mathf.Clamp(sounddata.bgmvolume, 0.001f, 1f);
        float sfxLin = Mathf.Clamp(sounddata.sfxvolume, 0.001f, 1f);

        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(bgmLin) * 20f : -80f;
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sfxLin) * 20f : -80f;

        audioMixer.SetFloat("Bgmvolume", bgmDb);
        audioMixer.SetFloat("Sfxvolume", sfxDb);

        // BGM 재생/일시정지
        if (sounddata.isBgmOn)
        {
            if (!bgmSource.isPlaying) bgmSource.Play();
        }
        else
        {
            if (bgmSource.isPlaying) bgmSource.Pause();
        }
    }
    public void PlayBgm(int index)
    {
        if (!sounddata.isBgmOn) return;
        var newClip = sounddata.bgmClips[index];
        if (bgmSource.clip == newClip && bgmSource.isPlaying) return;
        bgmSource.clip = newClip;
        bgmSource.Play();
    }


    public void PlaySfx(int index)
    {
        if (!sounddata.isSfxOn) return;
        sfxSource.PlayOneShot(sounddata.sfxClips[index]);
    }
#endregion
}