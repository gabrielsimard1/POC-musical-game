using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 2;
    EnemyMovement enemyMovement;

    public int currentHealth;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, float direction, float knockbackForce)
    {
        currentHealth -= damage;
        StartCoroutine(enemyMovement.KnockBack(direction, knockbackForce));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
