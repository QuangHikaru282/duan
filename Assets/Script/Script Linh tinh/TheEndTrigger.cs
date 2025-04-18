using UnityEngine;
using System.Collections;

public class TheEndTrigger : MonoBehaviour
{
    [Header("Objects to control")]
    public GameObject TheEnd;
    public GameObject exitButton;
    public MonoBehaviour playerControllerScript;

    [Header("Delay Settings")]
    public float delay = 2f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(HandleEndSequence());
        }
    }

    private IEnumerator HandleEndSequence()
    {
        yield return new WaitForSeconds(delay);

        if (TheEnd != null)
            TheEnd.SetActive(true);

        if (exitButton != null)
            exitButton.SetActive(true);

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;
    }
}
