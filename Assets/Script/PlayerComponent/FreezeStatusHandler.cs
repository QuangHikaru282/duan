using UnityEngine;

public class FreezeStatusHandler : MonoBehaviour
{
    [Header("Freeze Settings")]
    public GameObject freezeEffectPrefab;
    public Transform effectAnchor;

    [SerializeField] private float minFreezeTime = 1.2f;
    [SerializeField] private float maxFreezeTime = 3f;
    [SerializeField] private int inputToBreak = 8;

    private FreezeEffectController currentEffect;
    private float freezeStartTime;
    private bool isFrozen = false;
    private int inputCount = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 cachedVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void ApplyFreeze()
    {
        if (isFrozen) return;
        GetComponent<playerScript>().isFrozen = true;

        isFrozen = true;
        inputCount = 0;
        freezeStartTime = Time.time;

        cachedVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        if (animator != null)
        {
            animator.speed = 0f;
        }

        GameObject fx = Instantiate(freezeEffectPrefab, effectAnchor.position, Quaternion.identity, transform);
        currentEffect = fx.GetComponent<FreezeEffectController>();
    }

    private void Update()
    {
        if (!isFrozen) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Horizontal") != 0)
        {
            inputCount++;

            if (inputCount == 1 && currentEffect != null)
            {
                currentEffect.TriggerCrack();
            }
        }

        float elapsed = Time.time - freezeStartTime;
        if (elapsed >= maxFreezeTime || (elapsed >= minFreezeTime && inputCount >= inputToBreak))
        {
            Unfreeze();
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;
        GetComponent<playerScript>().isFrozen = false;


        if (currentEffect != null)
        {
            currentEffect.TriggerDestroy();
        }

        rb.isKinematic = false;

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }
}
