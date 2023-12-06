using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Game Object")]
    [SerializeField] BoxCollider2D feetCollider;

    [Header("Movement Speed")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float jumpSpeed = 5f;

    [Header("Knockback")]
    [SerializeField] int knockbackForce = 20;
    [SerializeField] float knockbackTime = .5f;

    bool isGrounded => feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

    Vector2 rawInput;
    Rigidbody2D myRigidbody;
    Animator animator;
    bool canMove = true;
    float yAxisKnockBack = .5f;
    float dashDuration = .2f;
    bool isDashing;
    bool dashInCooldown;
    float initialGravityScale;


    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialGravityScale = myRigidbody.gravityScale;
    }

    void FixedUpdate()
    {
        if (canMove && !isDashing) // constant movement was interfering with the knockback. Need to not take input into account to be able to knockback
        {
            Move();
        }
        if(dashInCooldown && isGrounded)
        {
            dashInCooldown = false; // reset dash cooldown
        }

    }

    void Move()
    {
        Vector2 playerVelocity = new(rawInput.x * moveSpeed, myRigidbody.velocity.y);
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
        if (!isGrounded || !canMove)
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

    void FlipSprite(bool hasHorizontalSpeed)
    {
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    public IEnumerator KnockBack(bool died)
    {
        canMove = false;
        animator.enabled = false;
        Vector2 knockbackDirection = new Vector2(-Mathf.Sign(myRigidbody.velocity.x), isGrounded ? yAxisKnockBack : 0);
        myRigidbody.velocity = knockbackDirection * knockbackForce;
        yield return new WaitForSeconds(knockbackTime);
        animator.enabled = !died;
        canMove = !died;
    }
    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isDashing", true);
        ToggleGravity(true);
        Vector2 dashDirection = new(Mathf.Sign(transform.localScale.x) * dashSpeed, 0f);

        if (isGrounded)
            myRigidbody.AddForce(dashDirection, ForceMode2D.Impulse);
        else
            myRigidbody.velocity = dashDirection; // if mid-air, lock y axis in place (otherwise if quick jump + dash, player dash upwards higher than jump height)

        yield return new WaitForSeconds(dashDuration);

        ToggleGravity(false);
        animator.SetBool("isDashing", false);
        isDashing = false;
        dashInCooldown = true;
    }

    void ToggleGravity(bool disableGravity)
    {
        // disable gravity so mid-air dash can stay mid-air
        // don't disable gravity if grounded, otherwise dash + jump makes it jump absurdly high
        if (disableGravity && !isGrounded)
            myRigidbody.gravityScale = 0;
        else
            myRigidbody.gravityScale = initialGravityScale;
    }


    bool DoesPlayerHaveHorizontalSpeed()
    {
        return Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; ;
    }
}
