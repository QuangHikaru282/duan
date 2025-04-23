using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockItem : MonoBehaviour
{
    public string skillToUnlock; // ví dụ "L", "E", "Q", "DoubleJump"

    [Header("DialogueBox Settings")]

    public DialogueBox dialogueBox;

    public DialogueData dialogueToPlay_1;
    public DialogueData dialogueToPlay_2;
    public DialogueData dialogueToPlay_3;
    public DialogueData dialogueToPlay_4;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SkillUnlockManager skillUnlockManager = collision.GetComponent<SkillUnlockManager>();
            if (skillUnlockManager != null)
            {
                skillUnlockManager.UnlockSkill(skillToUnlock);
                Destroy(gameObject); // Xoá vật phẩm sau khi nhặt
                TeleportPlayer();
            }
        }
    }
    public void TeleportPlayer()
    {
        if (skillToUnlock == "DoubleJump")
        {
            dialogueBox.StartDialogue(dialogueToPlay_1);
        }
        else if (skillToUnlock == "L")
        {
            dialogueBox.StartDialogue(dialogueToPlay_2);
        }
        else if (skillToUnlock == "E")
        {
            dialogueBox.StartDialogue(dialogueToPlay_3);
        }
        else if (skillToUnlock == "Q")
        {
            dialogueBox.StartDialogue(dialogueToPlay_4);
        }

    }
}

