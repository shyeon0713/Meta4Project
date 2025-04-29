using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMange_Title: MonoBehaviour
{
    public Button Bt1;  //새로하기
    public Button Bt2;  //이어하기
    public Button Bt3;  //엔딩확인
    public Button Bt4;  //설정
    public Button Bt5;  //종료하기

    // private SFXManager sfxmanager;  //SFXManager 

    // Start is called before the first frame update
    public void Start()
    {
        //  sfxmanager = FindObjectOfType<SFXManager>();
        Bt1 = GetComponent<Button>();
        Bt2 = GetComponent<Button>();
        Bt3 = GetComponent<Button>();
        Bt4 = GetComponent<Button>();
        Bt5 = GetComponent<Button>();

        Bt1.onClick.AddListener(Newfile);
        Bt2.onClick.AddListener(Continue);
        Bt3.onClick.AddListener(Checkending);
        Bt4.onClick.AddListener(Setting);
        Bt5.onClick.AddListener(Exit);
    }

    private void Newfile()
    {
        SceneManager.LoadScene("Activescene");
    }

    private void Continue()  // 세이브 파일이 없을 경우 회색으로 막아두기
    {
        SceneManager.LoadScene("Activescene");
    }

    private void Checkending()   
    {
        SceneManager.LoadScene("Activescene");
    }

    private void Setting()
    {
        SceneManager.LoadScene("Activescene");
    }

    private void Exit()
    {
        Application.Quit();  //앱 종료
    }
}