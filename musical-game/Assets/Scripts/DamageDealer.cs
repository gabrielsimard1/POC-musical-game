using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int contactDamage = 50;

    bool IsPlayerBeingHit(Collider2D collision) => gameObject.CompareTag("Enemy") && collision.gameObject.CompareTag("Player");

    bool IsEnemyBeingHitByBullet(Collider2D collision) => gameObject.CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Enemy");

    bool IsPlayerBulletHittingWall(Collider2D collision) => gameObject.CompareTag("PlayerBullet") && collision.gameObject.CompareTag("Ground");

    public float GetContactDamage()
    {
        return contactDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayerBeingHit(collision))
        {
            collision.gameObject.transform.parent?.gameObject.GetComponent<Health>()?.TakeDamage(contactDamage);
        } 
        else if (IsEnemyBeingHitByBullet(collision))
        {
            collision.gameObject.transform.parent?.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(contactDamage);
            Destroy(gameObject); // here we assume the enemy is being hit by a bullet so we destroy the bullet on impact
            // to adjust on sword dash
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag.Equals("PlayerBullet"))
        {
            Destroy(gameObject);

        }
    }
}
