using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] HealthBar healthBar;
    int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealthBarValue(currentHealth);
    }


    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

