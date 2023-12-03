using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform castPoint;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight = 15f;

    HashSet<string> playerTags = new() { "Player", "PlayerFeet" };

    EnemyMovement enemyMovement;
    Transform playerTransform;
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
        playerTransform = enemyMovement.GetPlayer();
    }

    bool IsGrounded()
    {
        return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    public void JumpToPlayer()
    {
        Vector2 endPos = playerTransform.position - castPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(castPoint.position, endPos, distToPlayer, ~LayerMask.GetMask("Enemy", "Ground"));

        if (hit.collider != null && playerTags.Contains(hit.collider.gameObject.tag))
        {
            float requiredJumpHeight = CalculateRequiredJumpHeight(playerTransform.position);
            float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

            if (jumpForceNeeded >= 2 && jumpForceNeeded <= jumpHeight && IsGrounded())
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

        if (hit.collider != null &&
            hit.collider.gameObject.CompareTag("Ground") &&
            !playerTags.Contains(hit.collider.gameObject.tag))
        {
            Vector2 obstacleAboveHeight = CheckObstacleAbove(hit.point, direction);
            if (obstacleAboveHeight != null)
            {
                float requiredJumpHeight = CalculateRequiredJumpHeight(obstacleAboveHeight);
                float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

                if (jumpForceNeeded <= jumpHeight && IsGrounded())
                {
                    Jump(1f);
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
        return Mathf.Max(0f, obstaclePosition.y - feetCollider.transform.position.y);
    }

    float CalculateJumpForceNeeded(float gravityScale, float jumpHeight)
    {
        return Mathf.Sqrt(2f * Mathf.Abs(gravityScale) * jumpHeight);
    }

    void Jump(float extraPushX = 1f)
    {
        float jumpForce = Mathf.Sqrt(2f * myRigidbody.gravityScale * jumpHeight);
        float unstuckForce = 2f; 
        myRigidbody.AddForce(new Vector2(-extraPushX * unstuckForce, jumpForce), ForceMode2D.Impulse);
    }

}
