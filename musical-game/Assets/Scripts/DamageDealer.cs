using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int contactDamage = 50;

    public float GetContactDamage()
    {
        return contactDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Enemy") && collision.gameObject.CompareTag("Player") && collision.gameObject.transform.parent)
        {
            collision.gameObject.transform.parent.gameObject.GetComponent<Health>()?.TakeDamage(contactDamage);
        } // todo adjust when enemies can shoot avec un OR 
    }
}
