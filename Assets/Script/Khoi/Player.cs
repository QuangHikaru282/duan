using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Destroy Destroy;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera1;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera2;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera3;
    

    public void SavePlayerData()
    {
 
    }

    public void LoadPlayerData()
    {
        
    }
    private void Awake()
    {
        
    }

   




    // Hiển thị phạm vi tấn công trong Scene view để tiện chỉnh sửa
    private void OnDrawGizmosSelected()
    {
        
    }



    // Hàm nhận sát thương
    public void TakeDamage(int damage)
    {
        
    }



    // Hàm xử lý khi Player chết
    private void Die()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Scene transition"))
        {
            virtualCamera1.gameObject.SetActive(false);
            virtualCamera2.gameObject.SetActive(true);
            virtualCamera3.gameObject.SetActive(false);
            Destroy.removeAll();
            Destroy(gameObject, 1f);
            Debug.Log("Scene transition");
        }
    }
}
