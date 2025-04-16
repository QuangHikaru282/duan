using UnityEngine;

public class FreezeEffectController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isCracking = false;
    private bool isDestroying = false;

    private float destroyDelay = 1.1f; 

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TriggerCrack()
    {
        if (isCracking || isDestroying) return;

        isCracking = true;
        animator.SetTrigger("InputTrigger"); 
    }

    public void TriggerDestroy()
    {
        if (isDestroying) return;

        isDestroying = true;
        animator.SetTrigger("isDestroyed");
        Destroy(gameObject, destroyDelay);
    }
}
