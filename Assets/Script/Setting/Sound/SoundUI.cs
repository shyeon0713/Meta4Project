using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SoundUI : MonoBehaviour
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

    public void Awake()
    {
        // 개발 중에 놓친 할당을 즉시 알리기
        //Debug.Log(bgmslider != null, "bgmSlider가 할당되지 않았음!");
        Debug.Assert(sfxslider != null, "sfxSlider가 할당되지 않았음!");
        Debug.Assert(bgmbutton != null, "bgmToggle이 할당되지 않았음!");
        Debug.Assert(sfxbutton != null, "sfxToggle이 할당되지 않았음!");
       
        // 버튼의 Image 컴포넌트를 캐시 (한 번만 찾는다)
        bgmButtonImage = bgmbutton.GetComponent<Image>();
        sfxButtonImage = sfxbutton.GetComponent<Image>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UI 를 현재 사운드 상태와 동기화 (초기값)
        SyncSoundUI();
    }

    private void OnEnable()
    {
        // 슬라이더 값이 바뀔 때 바로 볼륨을 적용 (저장은 나중에 할게요)
        bgmslider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxslider.onValueChanged.AddListener(OnSfxSliderChanged);

        // 버튼 클릭 시 ON/OFF 토글
        bgmbutton.onClick.AddListener(OnBgmButtonClicked);
        sfxbutton.onClick.AddListener(OnSfxButtonClicked);

        // SoundManager 가 보내는 이벤트를 구독 → 외부에서 값이 바뀌어도 UI가 따라감
        SoundSetting.OnBgmVolumeChanged += UpdateBgmSlider;
        SoundSetting.OnSfxVolumeChanged += UpdateSfxSlider;
        SoundSetting.OnBgmOnOffChanged += UpdateBgmIcon;
        SoundSetting.OnSfxOnOffChanged += UpdateSfxIcon;

    }

    private void OnDisable()
    {
       
        SoundSetting.OnBgmVolumeChanged -= UpdateBgmSlider;
        SoundSetting.OnSfxVolumeChanged -= UpdateSfxSlider;
        SoundSetting.OnBgmOnOffChanged -= UpdateBgmIcon;
        SoundSetting.OnSfxOnOffChanged -= UpdateSfxIcon;
    }

    private void OnBgmSliderChanged(float value) =>
        SoundSetting.Instance.SetBgmVolume(value, persist: false);

    private void OnSfxSliderChanged(float value) =>
        SoundSetting.Instance.SetSfxVolume(value, persist: false);

    private void OnBgmButtonClicked() => SoundSetting.Instance.BgmOnOff(!SoundSetting.Instance.IsBgmOn);
    private void OnSfxButtonClicked() => SoundSetting.Instance.SfxOnOff(!SoundSetting.Instance.IsSfxOn);




    private void UpdateBgmSlider(float newVal) => bgmslider.value = newVal;
    private void UpdateSfxSlider(float newVal) => sfxslider.value = newVal;
    private void UpdateBgmIcon(bool isOn) => bgmButtonImage.sprite = isOn ? bgmOnSprite : bgmOffSprite;
    private void UpdateSfxIcon(bool isOn) => sfxButtonImage.sprite = isOn ? sfxOnSprite : sfxOffSprite;


    private void SyncSoundUI()
    {
        // 현재 매니저에 저장된 값들을 UI에 그대로 반영한다.
        bgmslider.value = SoundSetting.Instance.BgmVolume;
        sfxslider.value = SoundSetting.Instance.SfxVolume;

        bgmButtonImage.sprite = SoundSetting.Instance.IsBgmOn ? bgmOnSprite : bgmOffSprite;
        sfxButtonImage.sprite = SoundSetting.Instance.IsSfxOn ? sfxOnSprite : sfxOffSprite;
    }

}