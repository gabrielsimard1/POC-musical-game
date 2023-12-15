using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] CircleCollider2D faceCollider;
    [SerializeField] BoxCollider2D feetCollider;

    [Header("Aggro")]
    [SerializeField, Tooltip("Only usable if Is Flying is set to false.")] bool canChase = true;
    [SerializeField, Tooltip("Only usable if Can Chase is set to false.")] bool isFlying;
    [SerializeField] Transform playerTargetTransform;
    [SerializeField, Tooltip("Value for enemy chase (both sides).")] float aggroRange = 4f;
    [SerializeField, Tooltip("Value for enemy chase once aggroed, chase doesn't stop unless distance between " +
        "player and enemy is higher, then tether will break. Should not be higher " +
        "than Aggro Range (can cause bugs).")]
    float tetherToPlayerLength = 4f;

    [Header("Movement")]
    [SerializeField, Tooltip("Only usable if Can Chase is set to false.")] bool verticalPatrol;
    [SerializeField] float patrolMoveSpeed = 3f;
    [SerializeField] float aggroMoveSpeed = 5f;
    [SerializeField] float returnToSpawnSpeed = 6f;
    [SerializeField, Range(0, 1)] float submergedMoveSpeedReduceCoef = 0.5f;
    
    [Header("Knockback")]
    [SerializeField, Min(0), Tooltip("Time between hit and the moment we check if it has hit the ground or reached end of knockback.")] float knockbackYeetTime = .5f;
    [SerializeField, Min(0), Tooltip("Time to wait until resume activity, after end of knockback.")] float knockbackDownTime = .5f;
    [SerializeField, Min(0), Tooltip("*Ground Enemies Only*\nY-axis propulsion for knockback.")] float yAxisKnockBack = 1f;

    EnemyJump enemyJump;
    Vector2 spawnPos;
    Rigidbody2D myRigidbody;

    bool isReturningToSpawn;
    float distToSpawnPos;
    float distToPlayer;
    const float distanceTolerance = 0.2f;
    float directionTowardsPlayer = 0f;
    bool canMove = true;

    bool canFlip = true;
    const float flipCooldown = 0.3f;
    float flipTimer;

    bool isSubmerged;
    float initialGravityScale;
    float initialDrag;
    float liquidsGravityScale = 2.5f;
    float liquidsDrag = 5f;
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
    public bool IsGrounded
    {
        get { return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")); }
    }


    public Transform GetPlayer() {  return playerTargetTransform; }
    public Rigidbody2D GetRigidbody() {  return myRigidbody; }
    public BoxCollider2D GetFeetCollider() {  return feetCollider; }
    public float GetDistToPlayer() {  return distToPlayer; }
    public float GetAggroRange() {  return aggroRange; }


    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        enemyJump = GetComponent<EnemyJump>();
        initialGravityScale = myRigidbody.gravityScale;
        initialDrag = myRigidbody.drag;
    }

    void Start()
    {
        spawnPos = transform.position;
        // disable canChase if isFlying is true
        canChase = !isFlying && canChase;
    }

    void Update()
    {
        IsSubmerged = myRigidbody.IsTouchingLayers(LayerMask.GetMask("Liquids"));
        if (!canFlip)
        {
            FlipCooldown();
        }
    }

    void FixedUpdate()
    {
        UpdateDistances();
        if (canMove)
            CurrentAction();
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
        else if (canChase && isReturningToSpawn)
            MoveToSpawn();
        else
            Patrol();
    }

    public void SetMoveSpeedViscosity()
    {
        myRigidbody.drag = IsSubmerged ? liquidsDrag : initialDrag;
        myRigidbody.gravityScale = IsSubmerged ? liquidsGravityScale : initialGravityScale;
    }

    bool PlayerIsInAggroRange()
    {
        return distToPlayer <= aggroRange;
    }

    void UpdateDistances()
    {
        distToPlayer = Vector2.Distance(faceCollider.transform.position, playerTargetTransform.position);
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
        if (!verticalPatrol)
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
        canFlip = false;
    }

    void ChasePlayer()
    {
        float chaseOrientation = Mathf.Sign(playerTargetTransform.position.x - faceCollider.transform.position.x);
        patrolMoveSpeed = Math.Abs(patrolMoveSpeed) * chaseOrientation;

        // flip once then put flip on cooldown to prevent very fast spin when right underneath player
        if (directionTowardsPlayer == 0 || canFlip)
        {
            directionTowardsPlayer = chaseOrientation;
            canFlip = false;
        }
        Move(directionTowardsPlayer * aggroMoveSpeed, true);
        transform.localScale = new(directionTowardsPlayer, 1, 1);
        enemyJump.JumpToPlayer();
    }

    void MoveToSpawn()
    {
        float directionTowardsSpawn = Mathf.Sign(spawnPos.x - transform.position.x);
        Move(directionTowardsSpawn * returnToSpawnSpeed, false);
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
        Move(patrolMoveSpeed);
    }

    void Move(float speed, bool isChasing = false)
    {
        if (IsSubmerged)
            speed *= submergedMoveSpeedReduceCoef;
        // if mid-air, do not add velocity (messes with jump velocity calculations and physics)
        // if flying, by default it wont chase player so no issues with jumping velocity
        if (IsGrounded || isFlying)
        {
            Vector2 movementAxis = verticalPatrol ? new(0,speed) : new(speed,0);
            myRigidbody.velocity = movementAxis;
            // check if is chasing to prevent double jump (since it's already jumping at player if so)
            if (!isChasing)
                enemyJump.JumpOnImminentCollision(Mathf.Sign(speed));
        }
    }
     
    public IEnumerator KnockBack(float direction, float knockbackForce)
    {
        if (knockbackForce > 0)
        {
            canMove = false;
            myRigidbody.velocity = CalculateKnockbackOrientation(direction) * knockbackForce;
            yield return new WaitForSeconds(knockbackYeetTime);
            if (!isFlying)
                yield return new WaitUntil(() => IsGrounded);
            myRigidbody.velocity = Vector3.zero;
            if (!canChase && distToSpawnPos > aggroRange) // if we knockback a passive enemy past aggroRange, change their patrol route
                spawnPos = transform.position;
            yield return new WaitForSeconds(knockbackDownTime);
            canMove = true;
        }
    }

    Vector2 CalculateKnockbackOrientation(float direction)
    {
        Vector2 knockbackDirection = new Vector2(Mathf.Sign(direction), IsGrounded ? yAxisKnockBack : 0);
        if (verticalPatrol)
            knockbackDirection = new Vector2(0, Mathf.Sign(direction));

        return knockbackDirection;
    }
}
