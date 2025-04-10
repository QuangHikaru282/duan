using UnityEngine;

public class BreakableWall : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AOE"))
        {
            Debug.Log("Breakable wall hit by AOE");
            Destroy(gameObject);
        }
    } // cach dung courutine

}
