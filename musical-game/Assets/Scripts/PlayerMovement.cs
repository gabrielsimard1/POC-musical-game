using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 rawInput;
    Rigidbody2D myRigidbody;

    [SerializeField] BoxCollider2D feetCollider;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    Animator animator;
    bool playerHasHorizontalSpeed;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        Move();
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
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
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

    bool DoesPlayerHaveHorizontalSpeed()
    {
        return Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; ;
    }
}
