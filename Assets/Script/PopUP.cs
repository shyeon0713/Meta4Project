using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopUP: MonoBehaviour
{
    public GameObject PopUpSetting;  // ����â

    // Update is called once per frame
    private void Start()
    {
        PopUpSetting.SetActive(false);
    }

}
