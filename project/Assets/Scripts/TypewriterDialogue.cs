using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class TypewriterDialogue : MonoBehaviour
{
    [Header("会話リスト（１要素＝１行）")]
    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("表示先の TextMeshProUGUI")]
    public TextMeshProUGUI dialogueText;

    [Header("１文字あたりの表示間隔 → 自動で文字送り")]
    public float typingSpeed = 0.05f;

    // 内部状態
    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    [System.Obsolete]
    void Start()
    {
        // Start() 内など
        var loader = FindObjectOfType<DialogueLoader>();
        dialogues = loader.GetDialogue("intro");
        ShowLine(0);

    }

    void Update()
    {
        // 左クリック（マウス左ボタン）またはスペースキー
        bool clicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool pressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        // 左クリック or スペースキー押下
        if (clicked || pressed)
        {
            // まだ文字送り中なら…即表示
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[currentIndex];
                typingCoroutine = null;
            }
            else
            {
                // 次の行へ
                currentIndex++;
                if (currentIndex < dialogues.Length)
                    ShowLine(currentIndex);
                else
                    OnAllDialoguesFinished();
            }
        }
    }

    /// <summary>指定した行をタイプライター表示でスタート</summary>
    void ShowLine(int index)
    {
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText(dialogues[index]));
    }

    /// <summary>１文字ずつ表示するコルーチン</summary>
    IEnumerator TypeText(string line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;  // 完了フラグ
    }

    /// <summary>全行表示後の処理（好きに拡張）</summary>
    void OnAllDialoguesFinished()
    {
        Debug.Log("▶▶▶ セリフがすべて表示されました");
        // 例：メニューを開く、シーンを切り替える など
    }
}