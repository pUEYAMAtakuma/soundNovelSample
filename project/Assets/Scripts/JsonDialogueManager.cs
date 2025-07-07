using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;       // TextMeshPro を使わないならこちら
using TMPro;
using UnityEngine.InputSystem;                // TextMeshPro を使う場合

[Serializable]
public class EventData
{
    public string type;      // "narration" or "dialogue"
    public string speaker;   // null もしくはキャラ名
    public string text;      // 表示したい本文
}

[Serializable]
public class SceneData
{
    public int id;
    public string name;
    public EventData[] events;
}

[Serializable]
public class ScenesRoot
{
    public SceneData[] scenes;
}

public class JsonDialogueManager : MonoBehaviour
{
    [Header("JSONファイル (Assets/Resources/dialogues.json)")]
    public TextAsset jsonFile;

    [Header("名前表示用 TextMeshProUGUI (Speaker)")]
    public TextMeshProUGUI nameText;

    [Header("本文表示用 TextMeshProUGUI")]
    public TextMeshProUGUI dialogueText;

    [Header("タイプライター速度")]
    public float typingSpeed = 0.05f;

    SceneData currentScene;
    EventData[] events;
    int eventIndex = 0;
    Coroutine typingCoroutine;

    void Start()
    {
        // 1. JSON読み込み
        var root = JsonUtility.FromJson<ScenesRoot>(jsonFile.text);
        if (root.scenes == null || root.scenes.Length == 0)
        {
            Debug.LogError("シーンが見つかりません");
            return;
        }

        // 2. 最初のシーンを選択（例として scenes[0]）
        currentScene = root.scenes[0];
        events = currentScene.events;

        // 3. イベント再生開始
        ShowEvent(eventIndex);
    }

    void Update()
    {

        // 左クリック（マウス左ボタン）またはスペースキー
        bool clicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool pressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        // 左クリック or スペースキー押下
        if (clicked || pressed)
        {
            if (typingCoroutine != null)
            {
                // 文字送り中 → 完全表示
                StopCoroutine(typingCoroutine);
                CompleteCurrentText();
            }
            else
            {
                // 次のイベントへ
                eventIndex++;
                if (eventIndex < events.Length)
                    ShowEvent(eventIndex);
                else
                    OnAllEventsFinished();
            }
        }
    }

    void ShowEvent(int idx)
    {
        var ev = events[idx];

        // スピーカー表示（nullなら空文字）
        nameText.text = string.IsNullOrEmpty(ev.speaker) ? "" : ev.speaker;

        // 本文クリア＆タイプ開始
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText(ev.text));
    }

    IEnumerator TypeText(string fullText)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    void CompleteCurrentText()
    {
        var ev = events[eventIndex];
        dialogueText.text = ev.text;
        typingCoroutine = null;
    }

    void OnAllEventsFinished()
    {
        Debug.Log("▶▶▶ 全イベント再生完了");
        // 必要に応じてシーン切り替えやメニュー起動
    }
}
