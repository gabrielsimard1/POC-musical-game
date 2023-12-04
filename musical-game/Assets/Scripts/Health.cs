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
        StartCoroutine(testTakeDamage());

    }

    void TakeDamage(int damage)
    {
        Debug.Log("taking damage");
        currentHealth -= damage;
        healthBar.SetHealthBarValue(currentHealth);
    }

    IEnumerator testTakeDamage()

    {
        yield return new WaitForSeconds(3);
        TakeDamage(50);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

