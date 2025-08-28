using System.Collections;
using UnityEngine;

using UnityEngine.Networking;


public class DayCheck : MonoBehaviour
{
    public bool IsInitialized { get; private set; } = false;


  
    private bool initialPosted = false;
    private int currentDay;
    public int CurrentDay
    {
        get { return currentDay; }
    }

    void Start()
    {


    }




}
