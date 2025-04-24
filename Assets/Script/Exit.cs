using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Application.Quit();  //�� ����
    }
}