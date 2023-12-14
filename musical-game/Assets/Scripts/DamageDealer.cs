using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int contactDamage = 50;
    [SerializeField] float knockbackForce = 5;

    HashSet<string> PLAYER_WEAPON_TAGS = new() { Tags.PLAYER_BULLET_TAG, Tags.PLAYER_SWORD_TAG };

    bool IsPlayerBeingHit(Collider2D collision) => gameObject.CompareTag(Tags.ENEMY_TAG) && collision.gameObject.CompareTag(Tags.PLAYER_TAG);

    bool IsEnemyBeingHitByPlayerWeapon(Collider2D collision) => PLAYER_WEAPON_TAGS.Contains(gameObject.tag) && collision.gameObject.CompareTag(Tags.ENEMY_TAG);


    public float GetContactDamage()
    {
        return contactDamage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnemyBeingHitByPlayerWeapon(collision))
        {
            HandleEnemyHitByPlayerWeapon(collision);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (IsPlayerBeingHit(collision))
        {
            collision.gameObject.transform.parent?.gameObject.GetComponent<Health>()?.TakeDamage(contactDamage);
        }
    }

    void HandleEnemyHitByPlayerWeapon(Collider2D collision)
    {
        collision.gameObject.transform.parent?.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(contactDamage, CalculateKnockbackDirection(), knockbackForce);
        if (gameObject.CompareTag(Tags.PLAYER_BULLET_TAG))
            Destroy(gameObject);
    }

    float CalculateKnockbackDirection()
    {
        float knockbackDirection = 0;
        if (gameObject.CompareTag(Tags.PLAYER_BULLET_TAG))
            knockbackDirection = gameObject.GetComponent<Rigidbody2D>().velocity.x; // direction projectile is coming from
        else if (gameObject.CompareTag(Tags.PLAYER_SWORD_TAG))
            knockbackDirection = gameObject.transform.parent.transform.localScale.x; // direction player is facing
        return knockbackDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.tag.Equals(Tags.PLAYER_BULLET_TAG))
        {
            Destroy(gameObject);
        }
    }
}
