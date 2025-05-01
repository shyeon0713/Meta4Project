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

    private void Start()
    {
        // �ʱ� ����
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

    //SoundManager.Instance.Play BGM(index); �� ȣ�� -> SoundData���� Ȱ��

    // SFX ���
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }


    //SoundManager.Instance.PlaySfx(index); �� ȣ�� -> SoundData���� Ȱ��

    public void ApplySettings()  //�ʱ� ����
    {
        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(sounddata.bgmvolume) * 20f : -80f;  // Slider �ʱ� ���� , AudioMixer ����Ȱ��
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sounddata.sfxvolume) * 20f : -80f;  // Slider �ʱ� ���� , AudioMixer ����Ȱ��

        audioMixer.SetFloat("BgmVolume", bgmDb);   // AudioMixer �ʱⰪ ����
        audioMixer.SetFloat("SfxVolume", sfxDb);   // AudioMixer �ʱⰪ ����
    }



}
