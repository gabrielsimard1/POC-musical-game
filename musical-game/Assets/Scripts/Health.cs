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
    PlayerMovement playerMovement;
    bool canTakeDamage = true;
    int currentHealth;
    Animator animator;
    Shooter shooter;

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public HealthBar GetHealthBar() 
    { 
        return healthBar; 
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        shooter = GetComponent<Shooter>();
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
            StartCoroutine(playerMovement.KnockBack());
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
        canTakeDamage = currentHealth > 0;
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
        shooter.SetCanShoot(false);
        canTakeDamage = false;
        playerMovement.SetCanMove(false);
        playerMovement.GetMyRigidbody().velocity = Vector3.zero;
        currentHealth = 0;
        StartCoroutine(playerMovement.DetachCameraFromPlayer());
        animator.SetBool("isDying", true);
        GameSession.Instance.ReloadScene();
    }

}

