using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SceneMange_Title: MonoBehaviour
{
    public Button Bt1;  //새로하기
    // Start is called before the first frame update
    public void Start()
    {
        SoundSetting.Instance.PlayBgm(6);  //6번 BGM
        
        Bt1 = GetComponent<Button>();
        Bt1.onClick.AddListener(Newfile);

    }

    private void Newfile()
    {
        SoundSetting.Instance.PlaySfx(4);  // sfx1
        SceneManager.LoadScene("Activescene");
    }
}