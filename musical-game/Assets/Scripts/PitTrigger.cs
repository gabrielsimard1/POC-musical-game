using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Health>()?.Die();
            collision.gameObject.GetComponent<PlayerMovement>()?.SetCanMove(false);
        }
    }
}
