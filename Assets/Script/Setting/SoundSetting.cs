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
        SceneManager.sceneLoaded += OnSceneLoaded; // ���� �ε�� �� ȣ��� �޼���
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)  //���� �ε�� �� ȣ��� �޼���
    {
        SetBgmForScene(scene.name); // �� �̸��� �´� BGM�� ����
    }



    // �� �̸��� �´� BGM�� ����
    public void SetBgmForScene(string sceneName)
    {
        int sceneIndex = GetSceneBgmIndex(sceneName); // ���� �´� BGM �ε����� ��ȯ
        PlayBgm(sceneIndex); // �ش� �ε����� BGM ���
    }


    // �� �̸��� �´� BGM �ε����� ��ȯ
    private int GetSceneBgmIndex(string sceneName)
    {
        // ���÷�, �� �̸��� ���� �ε����� �ٸ��� ����
        if (sceneName == "MainMenu") return 0; 
        if (sceneName == "Level1") return 1; 
        if (sceneName == "Level2") return 2;
        if (sceneName == "GameOver") return 3; 
        return 0; // �⺻ BGM
    }

    public void PlayBgm(int index)
    {
        bgmSource.clip = sounddata.bgmClips[index];
        bgmSource.Play();
    }

    // SFX ���
    public void PlaySfx(int index)
    {
        if (sounddata.isSfxOn) 
        {
            sfxSource.clip = sounddata.sfxClips[index];
            sfxSource.Play();
        }
    }


    //SoundManager.Instance.PlaySfx(indexvalue); �� ȣ��



    public void ApplySettings()  //�ʱ� ����
    {
        float bgmDb = sounddata.isBgmOn ? Mathf.Log10(sounddata.bgmvolume) * 20f : -80f;  // Slider �ʱ� ���� , AudioMixer ����Ȱ��
        float sfxDb = sounddata.isSfxOn ? Mathf.Log10(sounddata.sfxvolume) * 20f : -80f;  // Slider �ʱ� ���� , AudioMixer ����Ȱ��

        audioMixer.SetFloat("BgmVolume", bgmDb);   // AudioMixer �ʱⰪ ����
        audioMixer.SetFloat("SfxVolume", sfxDb);   // AudioMixer �ʱⰪ ����
    }



}
