using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    public GameObject Exitmenu;  

    // Update is called once per frame
    private void Start()
    {
        Exitmenu.SetActive(false);
    }
}