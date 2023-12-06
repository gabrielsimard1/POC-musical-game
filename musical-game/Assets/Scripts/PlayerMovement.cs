using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] BoxCollider2D feetCollider;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] int knockbackForce = 20;
    [SerializeField] float knockbackTime = .5f;

    bool isGrounded => feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

    Vector2 rawInput;
    Rigidbody2D myRigidbody;
    Animator animator;
    bool canMove = true;
    float yAxisKnockBack = .5f;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (canMove) // constant movement was interfering with the knockback. Need to not take input into account to be able to knockback
        {
            Move();
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

    bool DoesPlayerHaveHorizontalSpeed()
    {
        return Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; ;
    }
}
