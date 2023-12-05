using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] CircleCollider2D faceCollider;
    [SerializeField] BoxCollider2D feetCollider;

    [Header("Aggro")]
    [SerializeField] bool canChase;
    [SerializeField][Tooltip("Only usable if Can Chase is set to false.")] bool isVertical;
    [SerializeField] Transform playerTransform;
    [SerializeField][Tooltip("Value for enemy chase (both sides).")] float aggroRange = 4f;
    [SerializeField] [Tooltip("Value for enemy chase once aggroed, chase doesn't stop unless distance between " +
        "player and enemy is higher, then tether will break. Should not be higher " +
        "than Aggro Range (can cause bugs).")]
    float tetherToPlayerLength = 4f;

    [Header("Movement")]
    [SerializeField] float patrolMoveSpeed = 3f;
    [SerializeField] float aggroMoveSpeed = 5f;
    [SerializeField] float returnToSpawnSpeed = 6f;



    EnemyJump enemyJump;
    Vector2 spawnPos;
    Rigidbody2D myRigidbody;

    bool isReturningToSpawn;
    float distToSpawnPos;
    float distToPlayer;
    const float distanceTolerance = 0.2f;
    float directionTowardsPlayer = 0f;

    bool canFlip = true;
    const float flipCooldown = 0.3f;
    float flipTimer;


    public Transform GetPlayer() {  return playerTransform; }
    public Rigidbody2D GetRigidbody() {  return myRigidbody; }
    public BoxCollider2D GetFeetCollider() {  return feetCollider; }
    public float GetDistToPlayer() {  return distToPlayer; }
    public float GetAggroRange() {  return aggroRange; }


    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        enemyJump = GetComponent<EnemyJump>();
    }

    void Start()
    {
        spawnPos = transform.position;
    }

    void Update()
    {
        UpdateDistances();
        CurrentAction();
        if (!canFlip)
        {
            FlipCooldown();
        }
    }

    void FlipCooldown()
    {
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipCooldown)
        {
            canFlip = true;
            flipTimer = 0f;
        }
    }

    void CurrentAction()
    {
        if (canChase && PlayerIsInAggroRange() && !isReturningToSpawn)
            ChasePlayer();
        else if (isReturningToSpawn)
            MoveToSpawn();
        else
            Patrol();
    }

    bool PlayerIsInAggroRange()
    {
        return distToPlayer <= aggroRange;
    }

    void UpdateDistances()
    {
        distToPlayer = Vector2.Distance(faceCollider.transform.position, playerTransform.position);
        distToSpawnPos = Vector2.Distance(faceCollider.transform.position, spawnPos);

        if (distToSpawnPos > aggroRange && distToPlayer >= tetherToPlayerLength)
        {
            isReturningToSpawn = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (faceCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && canFlip)
            Flip();
    }
    void Flip()
    {
        patrolMoveSpeed = -patrolMoveSpeed;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        canFlip = false;
    }

    void ChasePlayer()
    {
        float chaseOrientation = Mathf.Sign(playerTransform.position.x - faceCollider.transform.position.x);
        patrolMoveSpeed = Math.Abs(patrolMoveSpeed) * chaseOrientation;

        // flip once then put flip on cooldown to prevent very fast spin when right underneath player
        if (directionTowardsPlayer == 0 || canFlip)
        {
            directionTowardsPlayer = chaseOrientation;
            canFlip = false;
        }
        MoveHorizontally(directionTowardsPlayer * aggroMoveSpeed, true);
        transform.localScale = new(directionTowardsPlayer, 1, 1);
        enemyJump.JumpToPlayer();
    }

    void MoveToSpawn()
    {
        float directionTowardsSpawn = Mathf.Sign(spawnPos.x - transform.position.x);
        MoveHorizontally(directionTowardsSpawn * returnToSpawnSpeed, false);
        transform.localScale = new(directionTowardsSpawn, 1, 1);

        if (distToSpawnPos <= 1f)
        {
            isReturningToSpawn = false;
            patrolMoveSpeed = Math.Abs(patrolMoveSpeed) * directionTowardsSpawn;
        }
    }

    void Patrol()
    {
        if (distToSpawnPos >= aggroRange - distanceTolerance && canFlip)
            Flip();
        MoveHorizontally(patrolMoveSpeed);
    }

    void MoveHorizontally(float speed, bool isChasing = false)
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myRigidbody.velocity = new Vector2(speed, 0);
            // check if is chasing to prevent double jump (since it's already jumping if so)
            if (!isChasing)
                enemyJump.JumpOnImminentCollision(Mathf.Sign(speed));
        }
    }
}
