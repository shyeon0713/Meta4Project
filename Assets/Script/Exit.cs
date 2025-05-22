using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Exit : MonoBehaviour
{
    public Button SwitchScene;  // ������ ��ư ������Ʈ�� ������ �� ����
    // Start is called before the first frame update
    void Start()
    {
        SwitchScene = GetComponent<Button>();
        SwitchScene.onClick.AddListener(GameExit);
    }
    private void GameExit()
    {
        SoundSetting.Instance.PlaySfx(1);  // sfx1
        Application.Quit();  //�� ����
    }
}