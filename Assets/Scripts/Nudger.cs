using UnityEngine;

public class Nudger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (gameObject.CompareTag(col.tag))
        {
            col.GetComponent<Nudgeable>().Nudge(transform.position.x);
        }
    }
}