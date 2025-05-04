using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 버튼 이미지 참조
        bgmButtonImage = bgmbutton.GetComponent<Image>();
        sfxButtonImage = sfxbutton.GetComponent<Image>();

        LoadSettings(); 

        // 슬라이더 초기값
        bgmslider.value = sounddata.bgmvolume;
        sfxslider.value = sounddata.sfxvolume;

        bgmslider.onValueChanged.AddListener(Set_BgmVolume);
        sfxslider.onValueChanged.AddListener(Set_SfxVolume);
        bgmbutton.onClick.AddListener(BgmOn);
        sfxbutton.onClick.AddListener(SfxOn);

        //오디오 믹서 값 적용 (슬라이더 초기값 설정 이후)
        SoundSetting.Instance.ApplySettings();

       
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
        PlayerPrefs.SetFloat("BgmVolume", sounddata.bgmvolume);
        PlayerPrefs.SetFloat("SfxVolume", sounddata.sfxvolume);
        PlayerPrefs.SetInt("BgmOn", sounddata.isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SfxOn", sounddata.isSfxOn ? 1 : 0);
        PlayerPrefs.Save();   
    }

    private void LoadSettings()  //세팅 가져오기 -> PlayerPrefs 활용
    {
        sounddata.bgmvolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
        sounddata.sfxvolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        sounddata.isBgmOn = PlayerPrefs.GetInt("BgmOn", 1) == 1;
        sounddata.isSfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
    }

}
