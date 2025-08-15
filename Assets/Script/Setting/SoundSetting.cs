using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class SoundSetting : MonoBehaviour
{
    #region 싱글톤
    public static SoundSetting Instance { get; private set; }  // 싱글톤 인스턴스 선언


    private void Awake()
    {//기존의 코드는 null일 경우를 조건으로 구현하여
     //다른 씬으로 이동 시에 SoundSetting 프리팹을 두 번 배치 -> 중복 배치
     //
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadSettings();
        ApplySettings();


        OnBgmVolumeChanged?.Invoke(BgmVolume);
        OnSfxVolumeChanged?.Invoke(SfxVolume);
        OnBgmOnOffChanged?.Invoke(IsBgmOn);
        OnSfxOnOffChanged?.Invoke(IsSfxOn);

    }

    private void OnDestroy()
    {
        // 구독 해제 – 메모리 누수 방지
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion


    #region audio mixer
    [Header("Audio")]

    public AudioMixer audioMixer;   // 변수 : Bgmvolume , Sfxvolume

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Data")]
    public SoundData sounddata;  //ScriptableObject인 SoundData.cs 참조
    // 초기 셋팅 값 지정되어있음
    #endregion


    #region Runtime Data (public read‑only)  
    // 플레이어 설정을 저장
    public float BgmVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;
    public bool IsBgmOn { get; private set; } = true;
    public bool IsSfxOn { get; private set; } = true;
    #endregion

    #region PlayerPrefs
    private const string KEY_BGM_VOL = "BgmVolume";
    private const string KEY_SFX_VOL = "SfxVolume";
    private const string KEY_BGM_ON = "BgmOn";
    private const string KEY_SFX_ON = "SfxOn";
    #endregion

    #region Events-> static event로 선언된 델리게이트 변수
    public static event Action<float> OnBgmVolumeChanged;   // linear 0‑1
    public static event Action<float> OnSfxVolumeChanged;
    public static event Action<bool> OnBgmOnOffChanged;
    public static event Action<bool> OnSfxOnOffChanged;
    #endregion

    #region 설정 Load/Save  
    
    void LoadSettings()  //Prefs 활용
    {
        BgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOL, 1f);
        SfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);
        IsBgmOn = PlayerPrefs.GetInt(KEY_BGM_ON, 1) == 1;
        IsSfxOn = PlayerPrefs.GetInt(KEY_SFX_ON, 1) == 1;
    }

    void SaveSettings()   // 설정 저장, Prefs 활용
    {
        PlayerPrefs.SetFloat(KEY_BGM_VOL, BgmVolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOL,SfxVolume);
        PlayerPrefs.SetInt(KEY_BGM_ON, IsBgmOn ? 1 : 0);
        PlayerPrefs.SetInt(KEY_SFX_ON, IsSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    #endregion


    #region  외부 호출 API -> Playerprefs API 호출
    // 볼륨 조정
    public void SetBgmVolume(float range, bool persist = true)
    {  // range = 볼륨 값 , persist = 해당 설정을 유지할 것 인가
        BgmVolume = Mathf.Clamp01(range);  // Clamp01 : 0 ~ 1 사이의 값만 선택하도록
        // 범위를 지정해놓음
        ApplyBgmVolume();  // 지정해놓은 볼륨 적용
        OnBgmVolumeChanged?.Invoke(BgmVolume); // ?. :Unity6 도입
                                               // 좌측객체가 null인지 검사 , 
                                               //null 이면 전체표현식을 아무일도 하지 않고 넘김
                                               //구독자가 없을 경우 호출 X
        if (persist)   // 설정을 유지할 경우, 설정 저장
        {
            SaveSettings();
        }

    }

    public void SetSfxVolume(float range, bool persist = true)
    {// range = 볼륨 값 , persist = 해당 설정을 유지할 것 인가
        SfxVolume = Mathf.Clamp01(range);// Clamp01 : 0 ~ 1 사이의 값만 선택하도록
        // 범위를 지정해놓음
        ApplySfxVolume();// 지정해놓은 볼륨 적용
        OnSfxVolumeChanged?.Invoke(SfxVolume);

        if (persist)
        {
            SaveSettings();
        }
    }

    // On/Off 조정

    public void BgmOnOff(bool on, bool persist = true)
    {// on = 재생여부 , persist = 해당 설정을 유지할 것 인가
        IsBgmOn = on;
        ApplyBgmOnOff();
        OnBgmOnOffChanged?.Invoke(IsBgmOn);

        if (persist)
        {
            SaveSettings();
        }
    }

    public void SfxOnOff(bool on, bool persist = true)
    {// on = 재생여부 , persist = 해당 설정을 유지할 것 인가
        IsSfxOn = on;
        ApplySfxOnOff();
        OnSfxOnOffChanged?.Invoke(IsSfxOn);

        if (persist)
        {
            SaveSettings();
        }
    }

    // Play
    public void PlayBgm(int index)
    {
        if (!IsBgmOn || sounddata == null) return;

        AudioClip clip = sounddata.bgmClips[index];  //sounddata에 설정된 브금 지정
        if(bgmSource.clip == clip && bgmSource.isPlaying) return;  //클립과 동일한 클립이고 현재 재생설정이 되어있으면 값 반환

        bgmSource.clip = clip;  // 클립 지정
        bgmSource.Play(); //bgm 재생
    }

    public void PlaySfx(int index)
    {
        if (!IsSfxOn || sounddata == null) return;
        sfxSource.PlayOneShot(sounddata.sfxClips[index]);
        // PlayOneShot : 동시다발 재생 가능
        // 별도 오디오 채널을 만들 필요 없이 즉시 재생

    }

    #endregion


    #region 로직 적용
    public void ApplySettings()   //초기 셋팅
    {
        ApplyBgmVolume();
        ApplySfxVolume();
        ApplyBgmOnOff();
        ApplySfxOnOff();
    }

    private void ApplyBgmVolume() => audioMixer.SetFloat("Bgmvolume", LinearToDecibel(BgmVolume));
    private void ApplySfxVolume() => audioMixer.SetFloat("SfxVolume", LinearToDecibel(SfxVolume));
    #endregion
    private void ApplyBgmOnOff()
    {
        if (IsBgmOn && !bgmSource.isPlaying && bgmSource.clip != null)
            bgmSource.Play();
        else if (!IsBgmOn && bgmSource.isPlaying)
            bgmSource.Pause();
    }

    private void ApplySfxOnOff()
    {
      // PlayOneshot을 활용하여 재생하기 때문에 코드가 따로 필요없음
      //추후를 위해 남겨둠
    }


    #region Utility -> 직접 무음 지정 (출처 : GPT)
    private static float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f) return -80f;          // 완전 무음
        return 20f * Mathf.Log10(linear);
    }
    #endregion


    #region Application lifecycle – 저장 보장 (출처 : GPT)
    private void OnApplicationPause(bool pause)
    {
        // 앱이 백그라운드(또는 포커스를 잃을 때) 호출됩니다.
        // pause == true   → 애플리케이션이 **일시정지**(백그라운드) 상태
        // pause == false  → 다시 포그라운드(활성) 상태가 되면 호출됩니다.
        if (pause) SaveSettings();   // 일시정지 시 현재 설정을 파일에 기록
    }

    private void OnApplicationQuit()
    {
        // 애플리케이션이 완전히 **종료**될 때 호출됩니다.
        SaveSettings();              // 종료 직전에 한 번 더 저장해 데이터 손실 방지
    }
    #endregion
}