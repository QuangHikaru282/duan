using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Image portraitLeft;
    public Image portraitRight;
    public float textSpeed = 0.05f;

    private DialogueData currentDialogue;
    private int index;

    [Header("ChoicePanel Settings")]
    public GameObject choicePanel;
    public Button[] choiceButtons;

    private int currentChoiceIndex = 0;


    public void StartDialogue(DialogueData dialogue)
    {
        GameState.isDialoguePlaying = true;
        currentDialogue = dialogue;
        index = 0;
        textComponent.text = string.Empty;
        gameObject.SetActive(true);
        UpdatePortrait();
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        // Nếu đang hiển thị lựa chọn thì không cho đọc tiếp thoại
        if (choicePanel.activeSelf)
        {
            HandleChoiceNavigation();
            return;
        }


        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (textComponent.text == currentDialogue.lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = currentDialogue.lines[index];
            }
        }
    }
    void HandleChoiceNavigation()
    {
        int maxIndex = currentDialogue.choices.Length - 1;

        // Di chuyển lên
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentChoiceIndex = (currentChoiceIndex - 2 + currentDialogue.choices.Length) % currentDialogue.choices.Length;
            UpdateChoiceSelection();
        }

        // Di chuyển xuống
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentChoiceIndex = (currentChoiceIndex + 2) % currentDialogue.choices.Length;
            UpdateChoiceSelection();
        }
        // Di chuyển lên
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentChoiceIndex = (currentChoiceIndex - 1 + currentDialogue.choices.Length) % currentDialogue.choices.Length;
            UpdateChoiceSelection();
        }

        // Di chuyển xuống
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentChoiceIndex = (currentChoiceIndex + 1) % currentDialogue.choices.Length;
            UpdateChoiceSelection();
        }

        // Chọn bằng Enter hoặc Space
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnChoiceSelected(currentChoiceIndex);
        }
    }
    void UpdateChoiceSelection()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i == currentChoiceIndex)
            {
                choiceButtons[i].Select(); // Tự động focus vào nút đó
            }
        }
    }


    IEnumerator TypeLine()
    {
        foreach (char c in currentDialogue.lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed); // DÙNG REALTIME
        }
    }

    void NextLine()
    {
        if (index < currentDialogue.lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            UpdatePortrait();
            StartCoroutine(TypeLine());
        }
        else
        {
            if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
            {
                ShowChoices(); // Hiển thị các nút chọn
            }
            else
            {
                EndDialogue();
            }
        }
    }
    void ShowChoices()
    {
        choicePanel.SetActive(true);
        currentChoiceIndex = 0;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentDialogue.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.choices[i].choiceText;

                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateChoiceSelection(); // Gọi để focus lựa chọn đầu tiên
    }

    void OnChoiceSelected(int index)
    {
        choicePanel.SetActive(false);

        DialogueData next = currentDialogue.choices[index].nextDialogue;

        if (next != null)
        {
            StartDialogue(next);
        }
        else
        {
            StartCoroutine(EndDialogueDelayed()); // Thoát nếu không có đoạn hội thoại tiếp theo
        }
    }
    IEnumerator EndDialogueDelayed()
    {
        yield return null; // chờ 1 frame
                           // hoặc yield return new WaitForSeconds(0.1f); để chắc chắn hơn

        EndDialogue();
    }

    void EndDialogue()
    {
        GameState.isDialoguePlaying = false;
        gameObject.SetActive(false);
        currentDialogue = null;
        portraitLeft.enabled = false;
        portraitRight.enabled = false;
    }

    void UpdatePortrait()
    {
        bool isPlayer = currentDialogue.isPlayerSpeaking[index];
        Sprite portrait = currentDialogue.portraits[index];

        if (isPlayer)
        {
            portraitRight.sprite = portrait;
            portraitRight.enabled = true;
            portraitLeft.enabled = false;
        }
        else
        {
            portraitLeft.sprite = portrait;
            portraitLeft.enabled = true;
            portraitRight.enabled = false;
        }
    }
}
