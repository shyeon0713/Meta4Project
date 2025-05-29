using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    [SerializeField]
    [Header("UI Component")]
    public Slider bgmslider;
    public Slider sfxslider;
    public Button bgmbutton;
    public Button sfxbutton;


    [Header("Change sprite")]
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

        //UI 초기값 동기화
        bgmslider.value = SoundSetting.Instance.sounddata.bgmvolume;
        sfxslider.value = SoundSetting.Instance.sounddata.sfxvolume;
        ChangesIcons(); // 버튼 이미지 교체

        //slider 이벤트 리스너 추가
        bgmslider.onValueChanged.AddListener(SoundSetting.Instance.SetBgmVolume);
        sfxslider.onValueChanged.AddListener(SoundSetting.Instance.SetSfxVolume);


        //button 이벤트 리스너 추가
        bgmbutton.onClick.AddListener(() =>
        {
            SoundSetting.Instance.BgmOnOff();
            ChangesIcons();  //버튼 이미지 교체
        });
        sfxbutton.onClick.AddListener(() =>
        {
            SoundSetting.Instance.SfxOnOff();
            ChangesIcons(); //버튼 이미지 교체
        });
    }

    private void ChangesIcons()
    {
        var data = SoundSetting.Instance.sounddata;


        bgmslider.value = data.bgmvolume;
        sfxslider.value = data.sfxvolume;


        bgmButtonImage.sprite = data.isBgmOn ? bgmOnSprite : bgmOffSprite;
        sfxButtonImage.sprite = data.isSfxOn ? sfxOnSprite : sfxOffSprite;
    }

}