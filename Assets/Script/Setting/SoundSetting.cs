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
        SoundSetting.Instance.PlayBgm(6);  //6번 BGM
        // 초기 설정
        ApplySettings();
    }

    public void PlayBgm(int index)
    {
        if (!sounddata.isBgmOn) return;

        AudioClip newClip = sounddata.bgmClips[index];

        // 현재 재생 중인 클립과 같고, 이미 재생 중이면 중복 재생 방지  0507수정
        if (bgmSource.clip == newClip && bgmSource.isPlaying)
            return;

        bgmSource.clip = newClip;
        bgmSource.Play();
    }

    /*
    public void PlayBgm(int index)
    {
        if (sounddata.isBgmOn)
        {
            bgmSource.clip = sounddata.bgmClips[index];
            bgmSource.Play();
        }
    }
*/
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
        float bgmLinear = Mathf.Clamp(sounddata.bgmvolume, 0.001f, 1f);
        float sfxLinear = Mathf.Clamp(sounddata.sfxvolume, 0.001f, 1f);

        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(bgmLinear) * 20f : -80f;
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sfxLinear) * 20f : -80f;

        //Debug.Log($"[ApplySettings] BGM: {bgmLinear} -> {bgmDb} dB | SFX: {sfxLinear} -> {sfxDb} dB");

        audioMixer.SetFloat("Bgmvolume", bgmDb); //변수명 무조건 :Bgmvolume으로 하기
        audioMixer.SetFloat("Sfxvolume", sfxDb); //변수명 무조건 :Sfxvolume으로 하기
    }



}