using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIUI : MonoBehaviour
{
    [[Header("AINPC_UIList")]
    public TMP_Text speaker;
    public TMP_Text NPCscript;
    public TMP_InputField PlayerInput;
    public Button Inputbutton;

    private void Start()
    {
        Inputbutton.onClick.AddListener(InputSend);
        StartCoroutine(DialogueAPI)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
