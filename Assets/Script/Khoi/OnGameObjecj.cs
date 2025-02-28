using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameObject : MonoBehaviour
{
    [SerializeField] private Animator outroAnimator;
    [SerializeField] private GameObject a;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Continued"))
        {
            StartCoroutine(PlayOutroAnimation());
        }
    }

    private IEnumerator PlayOutroAnimation()
    {
        a.SetActive(true);
        yield return new WaitUntil(() => outroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
}
