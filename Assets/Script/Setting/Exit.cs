using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Exit : MonoBehaviour
{
    public Button SwitchScene;  // 생성한 버튼 컴포넌트를 연결해 둘 변수
    // Start is called before the first frame update
    void Start()
    {
        SwitchScene = GetComponent<Button>();
        SwitchScene.onClick.AddListener(GameExit);
    }
    private void GameExit()
    {
        SoundSetting.Instance.PlaySfx(1);  // sfx1
        Application.Quit();  //앱 종료
    }
}