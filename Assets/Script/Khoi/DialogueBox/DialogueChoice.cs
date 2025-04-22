using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;                  // Nội dung hiển thị lên nút
    public DialogueData nextDialogue;         // Nếu null → thoát
}