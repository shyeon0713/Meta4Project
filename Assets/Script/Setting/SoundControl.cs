using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    private static SoundControl instance;  //�̱��� Ȱ��

    public SoundData sounddata;
    public Slider bgmslider;
    public Slider sfxslider;

    public Button bgmbutton;
    public Button sfxbutton;

    //�̹��� ��ü
    public Sprite bgmOnSprite;
    public Sprite bgmOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private Image bgmButtonImage;
    private Image sfxButtonImage;

    void Awake()  //�� �����, ���� ����ȭ 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // ���� ���� ���� �� ����
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ��ư �̹��� ����
        bgmButtonImage = bgmbutton.GetComponent<Image>();
        sfxButtonImage = sfxbutton.GetComponent<Image>();

        LoadSettings();
        SyncUIToData();  // �����̴�, ��ư �̹��� ������Ʈ

       //�̺�Ʈ ������ �߰�
         bgmslider.onValueChanged.AddListener(Set_BgmVolume);
         sfxslider.onValueChanged.AddListener(Set_SfxVolume);
        bgmbutton.onClick.AddListener(BgmOn);
        sfxbutton.onClick.AddListener(SfxOn);

        //����� �ͼ� �� ���� (�����̴� �ʱⰪ ���� ����)
        SoundSetting.Instance.ApplySettings();

    }

    void SyncUIToData()   //UI ����ȭ ���� �ذ� 0508���� 
    {
        bgmslider.value = sounddata.bgmvolume;  //bgm�� ��������
        sfxslider.value = sounddata.sfxvolume;  //sfx�� ��������
        UpdateButtonImages();
    }

    public void Set_BgmVolume(float vloume)
    {
        sounddata.bgmvolume = vloume;
        SoundSetting.Instance.ApplySettings();
        SaveSettings();
    }

    public void Set_SfxVolume(float vloume)
    {
        sounddata.sfxvolume = vloume;
        SoundSetting.Instance.ApplySettings();
        SaveSettings();
    }

    public void BgmOn()
    {
        sounddata.isBgmOn = !sounddata.isBgmOn;
        SoundSetting.Instance.ApplySettings();
        UpdateButtonImages(); 
        SaveSettings();       
    }

    public void SfxOn()
    {
        sounddata.isSfxOn = !sounddata.isSfxOn;
        SoundSetting.Instance.ApplySettings();
        UpdateButtonImages();
        SaveSettings();
    }

    private void UpdateButtonImages()
    {
        bgmButtonImage.sprite = sounddata.isBgmOn ? bgmOnSprite : bgmOffSprite;
        sfxButtonImage.sprite = sounddata.isSfxOn ? sfxOnSprite : sfxOffSprite;
    }

    private void SaveSettings()  //���� ���� -> PlayerPrefs Ȱ��
    {
        PlayerPrefs.SetFloat("Bgmvolume", sounddata.bgmvolume);
        PlayerPrefs.SetFloat("Sfxvolume", sounddata.sfxvolume);
        PlayerPrefs.SetInt("BgmOn", sounddata.isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SfxOn", sounddata.isSfxOn ? 1 : 0);
        PlayerPrefs.Save();   
    }

    private void LoadSettings()  //���� �������� -> PlayerPrefs Ȱ��
    {
        sounddata.bgmvolume = PlayerPrefs.GetFloat("Bgmvolume", 1f);
        sounddata.sfxvolume = PlayerPrefs.GetFloat("Sfxvolume", 1f);
        sounddata.isBgmOn = PlayerPrefs.GetInt("BgmOn", 1) == 1;
        sounddata.isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
    }


}
