using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform castPoint;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight = 15f;
    [SerializeField] float jumpCooldown = 1f;
    public bool canJump = true;
    float jumpTimer;

    HashSet<string> playerTags = new() { "Player", "PlayerFeet" };
    
    EnemyMovement enemyMovement;
    Transform player;
    float distToPlayer;
    Rigidbody2D myRigidbody;
    BoxCollider2D feetCollider;
    float aggroRange;


    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        myRigidbody = enemyMovement.GetRigidbody();
        feetCollider = enemyMovement.GetFeetCollider();
        aggroRange = enemyMovement.GetAggroRange();
    }


    void Update()
    {
        distToPlayer = enemyMovement.GetDistToPlayer();
        player = enemyMovement.GetPlayer();

        if (!canJump)
        {
            JumpCooldown();
        }
    }

    void JumpCooldown()
    {
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpCooldown)
        {
            canJump = true;
            jumpTimer = 0f;
        }
    }

    public void JumpToPlayer()
    {
        Vector2 endPos = player.position - castPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(castPoint.position, endPos, distToPlayer, ~LayerMask.GetMask("Enemy", "Ground"));

        if (hit.collider != null && playerTags.Contains(hit.collider.gameObject.tag))
        {
            float requiredJumpHeight = CalculateRequiredJumpHeight(player.position);
            float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

            if (jumpForceNeeded >= 2 &&
                jumpForceNeeded <= jumpHeight &&
                feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                Jump();
            }
        }
    }

    public void JumpOnImminentCollision(float direction)
    {
        // TODO: Figure out better way to calculate endPos than aggroRange / 2
        Vector2 endPos = new Vector2(direction, 0) * aggroRange / 2;
        RaycastHit2D hit = Physics2D.Raycast(castPoint.position, endPos, aggroRange / 2);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Ground"))
        {
            Vector2 obstacleAboveHeight = CheckObstacleAbove(hit.point, direction);
            if (obstacleAboveHeight != null)
            {
                float requiredJumpHeight = CalculateRequiredJumpHeight(obstacleAboveHeight);
                float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

                if (jumpForceNeeded <= jumpHeight && feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
                {
                    Jump();
                }
            }
        }
    }

    Vector2 CheckObstacleAbove(Vector2 hitPoint, float direction)
    {
        Vector2 castOffset = new(direction * 0.1f, 0);
        RaycastHit2D hitAbove = Physics2D.Raycast(hitPoint + castOffset, Vector2.up, LayerMask.GetMask("Ground"));

        while (hitAbove.collider != null && hitAbove.collider.CompareTag("Ground"))
        {
            // Move the raycast upward
            hitPoint = hitAbove.point + Vector2.up * 0.1f + castOffset;
            hitAbove = Physics2D.Raycast(hitPoint, Vector2.up, 0.1f);
        }

        return hitPoint;
    }

    float CalculateRequiredJumpHeight(Vector2 obstaclePosition)
    {
        float heightDifference = Mathf.Max(0f, obstaclePosition.y - feetCollider.transform.position.y);

        return heightDifference;
    }

    float CalculateJumpForceNeeded(float gravityScale, float jumpHeight)
    {
        return Mathf.Sqrt(2f * Mathf.Abs(gravityScale) * jumpHeight);
    }

    void Jump()
    {
        float jumpForce = Mathf.Sqrt(2f * myRigidbody.gravityScale * jumpHeight);
        myRigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }
}
