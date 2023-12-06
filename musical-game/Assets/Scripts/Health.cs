using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] HealthBar healthBar;
    [SerializeField] int invulnerabilityTime = 3;


    float blinkInterval = .1f;

    SpriteRenderer sprite;
    Rigidbody2D rigidBody;
    PlayerMovement playerMovement;
    bool canTakeDamage = true;
    int currentHealth;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            currentHealth -= damage;
            healthBar.SetHealthBarValue(currentHealth);
            StartCoroutine(playerMovement.KnockBack(currentHealth <= 0)); // so we can't move after we die
            StartCoroutine(StartInvulnerability());
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    IEnumerator StartInvulnerability()
    {
        canTakeDamage = false;
        StartCoroutine(Blink());
        yield return new WaitForSeconds(invulnerabilityTime);
        canTakeDamage = true;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    IEnumerator Blink()
    {
        float elapsedTime = 0;
        while (elapsedTime < invulnerabilityTime)
        {
            SetSpriteOpacity(.5f);
            yield return new WaitForSeconds(blinkInterval);
            SetSpriteOpacity(1);
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval * 2;
        }
        SetSpriteOpacity(1);
    }


    void SetSpriteOpacity(float opacity)
    {
        sprite.color = new Color(1, 1, 1, opacity);
    }

    public void Die()
    {
        GameSession.Instance.ReloadScene();
    }

}

