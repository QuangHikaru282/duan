﻿using System.Collections;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject player;
    public GameObject fire;
    public Animator animatorFire;
    public GameObject Decorate1;
    public GameObject Decorate2;
    public GameObject Decorate3;
    public GameObject Decorate4;
    public GameObject Decorate5;
    public GameObject DialogueBox;
    [SerializeField]
    private GameObject outro;
    [SerializeField]
    private Animator outroAnimator;
    [SerializeField]
    private GameObject loading;
    [SerializeField]
    public GameObject movingObject;
    public float moveSpeed = 2f; // Tốc độ di chuyển
    public float waveFrequency = 2f; // Tần số sóng (số lần lên xuống trong 1 giây)
    public float waveAmplitude = 0.5f; // Biên độ sóng (độ cao thấp)

    protected internal void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("loi 1");
        if (other.CompareTag("Player"))
        {
            Debug.Log("loi 2");
            CapsuleCollider2D playerCollider = other.GetComponent<CapsuleCollider2D>();
            BoxCollider2D groundCollider = GetComponent<BoxCollider2D>();

            if (playerCollider == null || groundCollider == null || fire == null)
            {
                Debug.Log("loi 4");
                Debug.LogWarning("Missing components or prefab!");
                return;
            }

            Vector2 startPosition = playerCollider.bounds.center;
            Vector2 targetPosition = groundCollider.bounds.center;
            Debug.Log("loi 6");
            if (movingObject != null)
            {
                Debug.Log("loi 5");
                StopCoroutine("MoveObjectToTarget");
                Destroy(movingObject); // Hủy đối tượng cũ nếu tồn tại
            }
            Debug.Log("loi 3");
            movingObject = Instantiate(fire, startPosition, Quaternion.identity);
            animatorFire = movingObject.GetComponent<Animator>();
            Debug.Log("loi m");
            StartCoroutine(ShowDecoratesInOrder());
            Debug.Log("loi n");
            StartCoroutine(MoveObjectToTarget(movingObject, targetPosition));
            
        }
    }

    private IEnumerator ShowDecoratesInOrder()
    {
        Decorate1.SetActive(true);
        yield return new WaitForSeconds(0.15f); 
        Decorate2.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        Decorate3.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        Decorate4.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        Decorate5.SetActive(true);
    }

    private IEnumerator MoveObjectToTarget(GameObject obj, Vector2 targetPosition)
    {
        // Di chuyển đối tượng đến targetPosition
        while (Vector2.Distance(obj.transform.position, targetPosition) > 0.1f)
        {
            obj.transform.position = Vector2.MoveTowards(obj.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Đảm bảo đối tượng chính xác ở targetPosition sau khi di chuyển xong
        obj.transform.position = targetPosition;

        // Bắt đầu di chuyển qua lại theo hình dấu ngã (~)
        StartCoroutine(MoveObjectWave(obj, targetPosition));
        yield return new WaitForSeconds(0.8f);
        DialogueBox.SetActive(true);
    }

    private IEnumerator MoveObjectWave(GameObject obj, Vector2 targetPosition)
    {
        
        float startTime = Time.time;

        while (movingObject  != null ) // Di chuyển vô tận
        {
            // Tạo hiệu ứng sóng theo trục Y
            float waveOffset = Mathf.Sin((Time.time - startTime) * waveFrequency) * waveAmplitude;
            obj.transform.position = new Vector2(targetPosition.x, targetPosition.y + waveOffset);

            yield return null; // Đợi frame tiếp theo
        }
        outro.SetActive(true);
        outroAnimator.SetBool("isOutro", true);
        StartCoroutine(WaitForAnimation(outroAnimator));
    }
    private IEnumerator WaitForAnimation(Animator animator)
    {
        // Đợi cho đến khi animation kết thúc
        yield return new WaitForSeconds(1.2f);
        animator.enabled = false;
        loading.SetActive(true);
        loading.SetActive(true);
    }
}
