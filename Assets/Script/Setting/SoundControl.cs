using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    private static SoundControl instance;  //싱글톤 활용

    public SoundData sounddata;
    public Slider bgmslider;
    public Slider sfxslider;

    public Button bgmbutton;
    public Button sfxbutton;

    //이미지 교체
    public Sprite bgmOnSprite;
    public Sprite bgmOffSprite;
    public Sprite sfxOnSprite;
    public Sprite sfxOffSprite;

    private Image bgmButtonImage;
    private Image sfxButtonImage;

    void Awake()  //씬 변경시, 설정 동기화 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 씬에 새로 생긴 건 삭제
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 버튼 이미지 참조
        bgmButtonImage = bgmbutton.GetComponent<Image>();
        sfxButtonImage = sfxbutton.GetComponent<Image>();

        LoadSettings();
        SyncUIToData();  // 슬라이더, 버튼 이미지 업데이트

       //이벤트 리스너 추가
         bgmslider.onValueChanged.AddListener(Set_BgmVolume);
         sfxslider.onValueChanged.AddListener(Set_SfxVolume);
        bgmbutton.onClick.AddListener(BgmOn);
        sfxbutton.onClick.AddListener(SfxOn);

        //오디오 믹서 값 적용 (슬라이더 초기값 설정 이후)
        SoundSetting.Instance.ApplySettings();

    }

    void SyncUIToData()   //UI 동기화 문제 해결 0508수정 
    {
        bgmslider.value = sounddata.bgmvolume;  //bgm값 가져오기
        sfxslider.value = sounddata.sfxvolume;  //sfx값 가져오기
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

    private void SaveSettings()  //세팅 저장 -> PlayerPrefs 활용
    {
        PlayerPrefs.SetFloat("Bgmvolume", sounddata.bgmvolume);
        PlayerPrefs.SetFloat("Sfxvolume", sounddata.sfxvolume);
        PlayerPrefs.SetInt("BgmOn", sounddata.isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SfxOn", sounddata.isSfxOn ? 1 : 0);
        PlayerPrefs.Save();   
    }

    private void LoadSettings()  //세팅 가져오기 -> PlayerPrefs 활용
    {
        sounddata.bgmvolume = PlayerPrefs.GetFloat("Bgmvolume", 1f);
        sounddata.sfxvolume = PlayerPrefs.GetFloat("Sfxvolume", 1f);
        sounddata.isBgmOn = PlayerPrefs.GetInt("BgmOn", 1) == 1;
        sounddata.isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
    }


}
