using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    public bool isMainPoint; // Đánh dấu đây là điểm chính (true) hay phụ (false)
    public TeleportPoint mainTeleportPoint; // Tham chiếu đến điểm chính (chỉ dùng cho điểm phụ)
    public GameObject fKeyPrefab; // Prefab "Fnew" để hiển thị nút F

    private GameObject fKeyInstance; // Biến lưu nút F khi hiển thị


    public BoxCollider2D detectionArea; // Bán kính phát hiện quái
    public LayerMask enemyLayer; // Layer của quái

    public Animator boom;
    public Animator animator_loading;
    public GameObject loading;

    public CameraShake CameraShake;

    public GameObject player_script;

    public GameObject UI;
    public float vibrate =0.001f;

    [Header("DialogueBox Settings")]
    
    public DialogueBox dialogueBox;

    public DialogueData dialogueToPlay_1;
    public DialogueData dialogueToPlay_2;
    public DialogueData dialogueToPlay_3;
    public DialogueData dialogueToPlay_4;
    private bool isDialogueBox_1 =false;
    private bool isDialogueBox_2 =false;
    private bool isDialogueBox_3 =false;
    private bool isDialogueBox_4 = false;


    // Khi player va chạm với điểm dịch chuyển
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hiển thị nút F ở vị trí của điểm dịch chuyển
            if (fKeyPrefab != null && fKeyInstance == null)
            {
                fKeyInstance = Instantiate(fKeyPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // Khi player rời khỏi điểm dịch chuyển
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Xóa nút F nếu đang hiển thị
            if (fKeyInstance != null)
            {
                Destroy(fKeyInstance);
                fKeyInstance = null;
            }
        }
    }

    // Hàm dịch chuyển player
    public void TeleportPlayer(GameObject player)
    {
        if (!isMainPoint && mainTeleportPoint != null)
        {
            StartCoroutine(HandleTeleportAfterAnimation(player));
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
        else if (!isDialogueBox_1)
        {
            dialogueBox.StartDialogue(dialogueToPlay_1);
            isDialogueBox_1 = true;
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
        else if (!isDialogueBox_2)
        {
            dialogueBox.StartDialogue(dialogueToPlay_2);
            isDialogueBox_2 = true;
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
        else if (!isDialogueBox_3)
        {
            dialogueBox.StartDialogue(dialogueToPlay_3);
            isDialogueBox_3 = true;
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
        else if (!isDialogueBox_4)
        {
            dialogueBox.StartDialogue(dialogueToPlay_4);
            isDialogueBox_4 = true;
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
        else
        {
            dialogueBox.StartDialogue(dialogueToPlay_2);
            Destroy(fKeyInstance);
            fKeyInstance = null;
        }
    }

    private IEnumerator HandleTeleportAfterAnimation(GameObject player)
    {
        // Kiểm tra quái vật trong vùng
        Collider2D[] enemiesInArea = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;

        int enemyCount = detectionArea.OverlapCollider(filter, enemiesInArea);

        if (enemyCount > 0)
        {
            Debug.Log("Không thể dịch chuyển: có quái ở gần!");
            yield break; // Dừng coroutine
        }
        else
        {
            GameState.isDialoguePlaying = true;
            // Bắt đầu animation
            boom.SetBool("boom", true);
            yield return new WaitForSeconds(2f);
            CameraShake.Instance.TriggerShake(4f, vibrate);
            
            loading.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            UI.SetActive(false);
            animator_loading.SetBool("loading", true);
            yield return new WaitForSeconds(2f);
            player.transform.position = mainTeleportPoint.transform.position;
            animator_loading.SetBool("loading", false);
            boom.SetBool("boom", false);
            yield return new WaitForSeconds(3f);
            animator_loading.SetBool("loading_2", true);
            
            UI.SetActive(true);
            yield return new WaitForSeconds(1f);
            animator_loading.SetBool("loading_2", false);
            CameraShake.RestoreZoneSettings();
            GameState.isDialoguePlaying = false;
            loading.SetActive(false);
        }
    }
}