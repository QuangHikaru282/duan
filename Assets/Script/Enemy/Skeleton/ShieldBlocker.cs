using UnityEngine;

public class ShieldBlocker : MonoBehaviour
{
    public BoxCollider2D colliderBlock;

    private void Awake()
    {
/*        if (colliderBlock != null)
            colliderBlock.enabled = false;*/
    }

    public void EnableBlockCollider()
    {
        Debug.Log("EnableBlockCollider called");
        if (colliderBlock != null)
            colliderBlock.enabled = true;
    }

    public void DisableBlockCollider()
    {
        if (colliderBlock != null)
            colliderBlock.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Skill"))
        {
            Destroy(other.gameObject);
        }
    }
}
