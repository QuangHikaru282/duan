using UnityEngine;
using System.Collections;

public class ManaPickup : MonoBehaviour
{
    [Header("Mana Settings")]
    public int manaAmount = 10;
    private bool isPickedUp = false;
    private AudioManager audioManager;
    private Animator animator;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPickedUp && collision.CompareTag("Player"))
        {
            isPickedUp = true;

            audioManager.PlaySFX(audioManager.manaClip);
            SkillManager.Instance.AddMana(manaAmount);

            if (animator != null)
            {
                animator.SetBool("isPickedUp", true);
            }

            Collider2D col2d = GetComponent<Collider2D>();
            if (col2d != null)
            {
                col2d.enabled = false;
            }

            StartCoroutine(DestroyAfterCollected(0.6f));
        }
    }

    IEnumerator DestroyAfterCollected(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
