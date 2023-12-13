using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Game Object")]
    [SerializeField] BoxCollider2D feetCollider;
    [SerializeField] ParticleSystem dashParticleSystem;

    [Header("Movement Speed")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float submergedMoveSpeed = 5f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float jumpSpeed = 5f;

    [Header("Knockback")]
    [SerializeField] int knockbackForce = 20;
    [SerializeField] float knockbackTime = .5f;

    bool IsGrounded => feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

    Health playerHealth;
    Vector2 rawInput;
    Rigidbody2D myRigidbody;
    Animator animator;
    WeaponController weaponController;
    Shooter shooter;

    bool DoesPlayerHaveHorizontalSpeed() => Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
    bool canMove = true;
    float yAxisKnockBack = .5f;
    
    float dashDuration = .2f;
    bool isDashing;
    bool dashInCooldown;
    
    bool isSubmerged;
    float initialGravityScale;
    float initialDrag;
    float liquidsGravityScale = 2f;
    float liquidsRunDrag = 15f;
    float liquidsDashDrag = 5f;

    CinemachineVirtualCamera vcam;
    float cameraDetachDelay = 0.3f;

    public bool IsSubmerged
    {
        get { return isSubmerged; }
        private set
        {
            if (isSubmerged != value)
            {
                isSubmerged = value;
                SetMoveSpeedViscosity();
            }
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }


    void Awake()
    {
        playerHealth = GetComponent<Health>();
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialGravityScale = myRigidbody.gravityScale;
        weaponController = GetComponent<WeaponController>();
        initialDrag = myRigidbody.drag;
        shooter = GetComponent<Shooter>();
    }

    void Update()
    {
        IsSubmerged = myRigidbody.IsTouchingLayers(LayerMask.GetMask("Liquids"));
    }

    void FixedUpdate()
    {
        if (canMove && !isDashing) // constant movement was interfering with the knockback. Need to not take input into account to be able to knockback
        {
            Move();
        }
        if((dashInCooldown && IsGrounded) || IsSubmerged)
        {
            dashInCooldown = false; // reset dash cooldown upon landing or if submerged
        }
    }

    public void SetMoveSpeedViscosity()
    {
        myRigidbody.drag = IsSubmerged ? liquidsRunDrag : initialDrag;
        myRigidbody.gravityScale = IsSubmerged ? liquidsGravityScale : initialGravityScale;
    }

    public IEnumerator DetachCameraFromPlayer()
    {
        yield return new WaitForSeconds(cameraDetachDelay);
        vcam.LookAt = null;
        vcam.Follow = null;
    }

    void Move()
    {
        Vector2 playerVelocity = new(rawInput.x * (IsSubmerged ? submergedMoveSpeed : moveSpeed), myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = DoesPlayerHaveHorizontalSpeed();
        animator.SetBool("isWalking", playerHasHorizontalSpeed);
        FlipSprite(playerHasHorizontalSpeed);
    }

    void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if ((!IsGrounded && !IsSubmerged) || !canMove)
            return;
        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }
    void OnDash(InputValue value)
    {
        if (!canMove || dashInCooldown)
            return;
        if (value.isPressed)
        {
            StartCoroutine(Dash());
        }
    }

    void OnSwapWeapon(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            weaponController.IncrementSelectedWeapon();
        } 
        else if (value.Get<float>() < 0)
        {
            weaponController.DecrementSelectedWeapon();
        }
    }

    void FlipSprite(bool hasHorizontalSpeed)
    {
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    public IEnumerator KnockBack()
    {
        canMove = false;
        shooter.SetCanShoot(false);
        animator.SetBool("isWalking", false);
        Vector2 knockbackDirection = new Vector2(-Mathf.Sign(myRigidbody.velocity.x), IsGrounded ? yAxisKnockBack : 0);
        myRigidbody.velocity = knockbackDirection * knockbackForce;
        yield return new WaitForSeconds(knockbackTime);
        canMove = playerHealth.GetCurrentHealth() > 0;
        shooter.SetCanShoot(true);
    }
    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isDashing", true);
        dashParticleSystem.Play();
        ToggleGravityDash(true);
        Vector2 dashDirection = new(Mathf.Sign(transform.localScale.x) * dashSpeed, 0f);

        if (IsGrounded)
            myRigidbody.AddForce(dashDirection, ForceMode2D.Impulse);
        else
            myRigidbody.velocity = dashDirection; // if mid-air, lock y axis in place (otherwise if quick jump + dash, player dash upwards higher than jump height)

        yield return new WaitForSeconds(dashDuration);

        ToggleGravityDash(false);
        dashParticleSystem.Stop();
        animator.SetBool("isDashing", false);
        isDashing = false;
        dashInCooldown = true;
    }

    void ToggleGravityDash(bool disableGravity)
    {
        // disable gravity so mid-air dash can stay mid-air
        // don't disable gravity if grounded, otherwise dash + jump makes it jump absurdly high
        myRigidbody.gravityScale = disableGravity && !IsGrounded ? 0 : initialGravityScale;

        if (disableGravity && IsSubmerged)
            myRigidbody.drag = liquidsDashDrag;
        else if (!disableGravity)
            SetMoveSpeedViscosity();
    }

    void OnFire(InputValue value)
    {
        if (shooter != null)
        {
            shooter.SetIsFiring(value.isPressed);
        }
    }
}
