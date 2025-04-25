using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
public class ScriptUI : MonoBehaviour
{
    public Button nextbutton;  //��ư Ȱ��ȭ

    private DialogueData dialogueData;  // json���� �ҷ��� ��� ������ ������ ����
    public TMP_Text npcnametext;
    public TMP_Text dialogueText;

    public Image npcImage;  //NPC �̹��� 
    public List<Sprite> npcSpriteList;
    public List<string> npcSpriteNames;

    float gray = 111f / 225f;  //���� ���� ���, NPC ȸ��ó���� ����

    private Dictionary<string, Sprite> npcSpriteDict = new();

    private int currentIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextbutton.onClick.AddListener(OutputScript);  // ��ư ����

        // JSON ���� �ε�
        string path = Path.Combine(Application.streamingAssetsPath, "MainStory.json");

        // DialogueData �ʱ�ȭ
        dialogueData = DialogueData.LoadFromJson(path);  // `LoadFromJson` ���!

        Debug.Log("Path to JSON: " + path);

        // ���� ��� Ȯ�� �� ������ ����
        if (dialogueData == null || dialogueData.dialogues == null || dialogueData.dialogues.Length == 0)
        {
            Debug.LogError("Dialogue data is empty or could not be loaded.");  //�ε尡 ���� ���� ���
            return;
        }

        // ��������Ʈ ��ųʸ� �ʱ�ȭ
        if (npcSpriteList.Count != npcSpriteNames.Count)
        {
            Debug.LogError("Sprite list and sprite name list do not match in length!");
            return;
        }

        for (int i = 0; i < npcSpriteList.Count; i++)
        {
            npcSpriteDict[npcSpriteNames[i]] = npcSpriteList[i];
        }

        OutputDialogue(currentIndex);
    }


    void OutputScript()  // ���� ��ư ������ ���� ��� ���
    {
        currentIndex++;

        if (dialogueData == null || dialogueData.dialogues == null)  //0424���� : null�� ��츦 ��������
        {
            Debug.LogError("Dialogue data or dialogues is null!");
            return;  // �� �̻� �������� ����
        }

        if (currentIndex < dialogueData.dialogues.Length)
        {
            OutputDialogue(currentIndex);
        }
        else  //������ ��� ���� ó��
        {
         //  nextbutton.interactable = false;  // ��ư ��Ȱ��ȭ
            dialogueText.text = ""; 
        }

    }


    void OutputDialogue(int index)
    {
        var line = dialogueData.dialogues[index];  // var: �����Ϸ��� ������ Ÿ���� �ڵ����� �߷��ϰ� ���ִ� Ű����
        npcnametext.text = line.speaker;
        dialogueText.text = line.text;

        npcImage.color = new Color(1f, 1f, 1f, 1f);  //�⺻�� -> ���

        if (line.speaker == "����")  //���밡 ���� ���, ����LD�� ���
        {
            npcImage.sprite = GetSpriteFromDict(line.sprite);

        }
        else  // ���� ���� ���, ����LD�� ȸ��
        {
            npcImage.sprite = GetSpriteFromDict(line.sprite);
            npcImage.color = new Color(gray, gray, gray, 1f); // ȸ�� ó��
        }
    }

    Sprite GetSpriteFromDict(string spriteName) 
      //JSON���� �Ѿ�� sprite �̸��� ���� Unity�� Sprite ��ü�� �ٲٱ�
    {
        if (npcSpriteDict.TryGetValue(spriteName, out Sprite sprite))
        //TryGetValue : Dictionary���� ���� �����ϴ��� �����ϰ� ������ ��� + null ���� ����
        //�����ϸ� �� ��ȯ, ������ ���� ���� ó�� ����
        {
            return sprite;
        }
        return null;
    }

}