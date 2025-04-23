using UnityEngine;
using System.Collections;

public class BreakableWall : MonoBehaviour
{
    [Header("VFX Settings")]
    public GameObject smokeEffectPrefab;
    private GameObject smokeEffectInstance;
    public GameObject burnEffectPrefab;
    private GameObject burnEffectInstance;
    private Animator animator;
    public AnimationClip destroywallAnim;
    public bool isDestroyWall =false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AOE"))
        {
            StartBurnEffect();
            StartSmokeEffect();
            StartCoroutine(DestroyAfterDelay(2f));
            isDestroyWall = true;
        }
    }

    private void StartSmokeEffect()
    {
        if (smokeEffectPrefab != null && smokeEffectInstance == null)
        {
            smokeEffectInstance = Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);
            smokeEffectInstance.transform.SetParent(this.transform, true);
        }
    }
    void StartBurnEffect()
    {
        if (burnEffectPrefab != null && burnEffectInstance == null)
        {
            burnEffectInstance = Instantiate(burnEffectPrefab, transform.position, Quaternion.identity);
            burnEffectInstance.transform.SetParent(this.transform, true);
        }
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {

        if (destroywallAnim != null && animator != null)
        {
            animator.Play(destroywallAnim.name, 0, 0f);
        }
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}
