using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange_Briefing : MonoBehaviour
{
    public Button Bt_Dialogue;

   // private SFXManager sfxmanager;  //SFXManager 

    // Start is called before the first frame update
    public void Start()
    {
     //  sfxmanager = FindObjectOfType<SFXManager>();

        Bt_Dialogue.onClick.AddListener(GotoDialogue);
    }

    private void GotoDialogue()
    {
       // SFXManager.Instance.PlaySFX("ChangeScene");  //È¿°úÀ½ Àç»ý
        //Debug.Log("ButtonClick");
        SceneManager.LoadScene("SaveLoad");
    }
}