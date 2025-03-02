using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameObject : MonoBehaviour
{
    [SerializeField] private Animator outroAnimator;
    [SerializeField] private Animator continuedAnimator;
    [SerializeField] private GameObject a;
    [SerializeField] private GameObject outro;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Continued"))
        {
            StartCoroutine(PlayOutroAnimation());
        }
    }

    private IEnumerator PlayOutroAnimation()
    {
        outro.SetActive(true);
        outroAnimator.SetBool("isOutro", true);

        yield return new WaitUntil(() => outroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        outroAnimator.SetBool("isOutro", false);
        outro.SetActive(false);

        a.SetActive(true);
        

        outro.SetActive(true);
        outroAnimator.SetBool("isIntro", true);

        yield return new WaitUntil(() => outroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        outro.SetActive(false);
        
        

        
    }
}
