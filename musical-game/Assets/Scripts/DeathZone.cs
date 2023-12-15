using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health playerHealth = collision.gameObject.GetComponent<Health>();

        if (collision.CompareTag(Tags.PLAYER_TAG) && playerHealth)
        {
            playerHealth.Die();
            playerHealth.GetHealthBar().SetHealthBarValue(0);
        }
    }

}
