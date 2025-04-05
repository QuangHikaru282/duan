using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWay : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(DisableCollision());
        }
    }

    IEnumerator DisableCollision()
    {
        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        effector.rotationalOffset = 180f; // Cho phép rơi xuống
        yield return new WaitForSeconds(0.5f);
        effector.rotationalOffset = 0f;   // Khóa lại
    }
}
