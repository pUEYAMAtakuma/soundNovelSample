using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueSet
{
    public List<string> intro;
    public List<string> chapter1;
}

public class DialogueLoader : MonoBehaviour
{
    public DialogueSet dialogues;

    void Awake()
    {
        var json = Resources.Load<TextAsset>("Dialogues/dialogues").text;
        dialogues = JsonUtility.FromJson<DialogueSet>(json);
    }

    // 例：TypewriterDialogue に渡す
    public string[] GetDialogue(string key)
    {
        var list = typeof(DialogueSet)
            .GetField(key)
            .GetValue(dialogues) as List<string>;
        return list.ToArray();
    }
}
