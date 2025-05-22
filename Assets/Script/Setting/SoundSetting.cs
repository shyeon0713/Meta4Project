using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundSetting : MonoBehaviour
{
    public static SoundSetting Instance { get; private set; }  // �̱��� �ν��Ͻ� ����

    public AudioMixer audioMixer;
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public SoundData sounddata;  //ScriptableObject ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  //���� ������ ������Ʈ�� �������� ���� ������ �ν��Ͻ��� ����
            DontDestroyOnLoad(gameObject);  //���� ��ȯ�Ǿ ������Ʈ�� ����
        }
        else
        {
            Destroy(gameObject);  
            return;
        }
    }

     void Start()
    {
        SoundSetting.Instance.PlayBgm(6);  //6�� BGM
        // �ʱ� ����
        ApplySettings();
    }

    public void PlayBgm(int index)
    {
        if (!sounddata.isBgmOn) return;

        AudioClip newClip = sounddata.bgmClips[index];

        // ���� ��� ���� Ŭ���� ����, �̹� ��� ���̸� �ߺ� ��� ����  0507����
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
    //SoundManager.Instance.PlayBGM(index); �� ȣ�� -> SoundData���� Ȱ��

    // SFX ���
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }



    //SoundControl.Instance.PlaySfx(index); �� ȣ�� -> SoundData���� Ȱ��
    public void ApplySettings()  //�ʱ� ����
    {
        float bgmLinear = Mathf.Clamp(sounddata.bgmvolume, 0.001f, 1f);
        float sfxLinear = Mathf.Clamp(sounddata.sfxvolume, 0.001f, 1f);

        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(bgmLinear) * 20f : -80f;
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sfxLinear) * 20f : -80f;

        //Debug.Log($"[ApplySettings] BGM: {bgmLinear} -> {bgmDb} dB | SFX: {sfxLinear} -> {sfxDb} dB");

        audioMixer.SetFloat("Bgmvolume", bgmDb); //������ ������ :Bgmvolume���� �ϱ�
        audioMixer.SetFloat("Sfxvolume", sfxDb); //������ ������ :Sfxvolume���� �ϱ�
    }



}