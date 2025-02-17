using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    public GameObject imagePrefab;
    private GameObject imageA;
    private GameObject imageB;

    void Start()
    {
        Transform parent = transform;
        imageA = parent.GetChild(0).gameObject;
        imageB = parent.GetChild(1).gameObject;
    }

    protected internal void AddHeart()
    {
        // Tạo Image mới
        GameObject newImage = Instantiate(imagePrefab, transform);

        // Xác định vị trí giữa img A và B
        newImage.transform.SetSiblingIndex(1);
    }

    protected internal void RemoveHeart()
    {
        // Điều kiện còn máu
        if (transform.childCount > 0)
        {
            // Lấy đối tượng cuối cùng
            Transform lastChild = transform.GetChild(transform.childCount - 1);

            // Lấy Animator của đối tượng cuối cùng ( làm hiệu ứng mất máu )
            Animator heartAnimator = lastChild.GetComponent<Animator>();

            StartCoroutine(PlayRemoveHeartAnimation(lastChild, heartAnimator));
        }
    }

    private IEnumerator PlayRemoveHeartAnimation(Transform heart, Animator heartAnimator)
    {
        heartAnimator.SetBool("Hurt", true);

        yield return null;

        AnimatorStateInfo stateInfo = heartAnimator.GetCurrentAnimatorStateInfo(0);

        // Chờ cho đến khi hoạt frame kết thúc
        while (stateInfo.normalizedTime < 1f)
        {
            stateInfo = heartAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Xóa Heart
        Destroy(heart.gameObject);
    }
}
