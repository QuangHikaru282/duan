using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockItem : MonoBehaviour
{
    public string skillToUnlock; // ví dụ "L", "E", "Q", "DoubleJump"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SkillUnlockManager skillUnlockManager = collision.GetComponent<SkillUnlockManager>();
            if (skillUnlockManager != null)
            {
                skillUnlockManager.UnlockSkill(skillToUnlock);
                Destroy(gameObject); // Xoá vật phẩm sau khi nhặt
            }
        }
    }
}

