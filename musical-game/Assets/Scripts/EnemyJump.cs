using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform jumpOriginCastPoint;

    [Header("Jump Variables")]
    [SerializeField] float jumpHeight = 15f;

    HashSet<string> playerTags = new() { Tags.PLAYER_TAG, Tags.PLAYER_FEET_TAG };

    EnemyMovement enemyMovement;
    Transform playerTransform;
    float distToPlayer;
    Rigidbody2D myRigidbody;
    BoxCollider2D feetCollider;
    float aggroRange;
    int LayersToIgnoreJump => ~LayerMask.GetMask("Enemy", "Liquids", "Spikes");


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

    bool CollidesWithWall(RaycastHit2D hit)
    {
        return hit.collider.gameObject.CompareTag(Tags.GROUND_TAG);
    }

    public void JumpToPlayer()
    {
        Vector2 endPos = playerTransform.position - jumpOriginCastPoint.position;
        RaycastHit2D hit = Physics2D.Raycast(jumpOriginCastPoint.position, endPos, distToPlayer, LayersToIgnoreJump);

        if (hit.collider != null && 
            (CollidesWithWall(hit) || playerTags.Contains(hit.collider.gameObject.tag)))
        {
            float requiredJumpHeight = CalculateRequiredJumpHeight(playerTransform.position);
            float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

            if (jumpForceNeeded >= 2 && jumpForceNeeded <= jumpHeight && enemyMovement.IsGrounded)
            {
                Jump();
            }
        }
    }

    public void JumpOnImminentCollision(float direction)
    {
        // TODO: Figure out better way to calculate endPos than aggroRange / 2
        Vector2 endPos = new Vector2(direction, 0) * aggroRange / 2;
        RaycastHit2D hit = Physics2D.Raycast(jumpOriginCastPoint.position, endPos, aggroRange / 2, LayersToIgnoreJump);

        if (hit.collider != null &&
            CollidesWithWall(hit) &&
            !playerTags.Contains(hit.collider.gameObject.tag))
        {
            Vector2 obstacleAboveHeight = CheckObstacleAbove(hit.point, direction);
            if (obstacleAboveHeight != null)
            {
                float requiredJumpHeight = CalculateRequiredJumpHeight(obstacleAboveHeight);
                float jumpForceNeeded = CalculateJumpForceNeeded(myRigidbody.gravityScale, requiredJumpHeight);

                if (jumpForceNeeded <= jumpHeight && enemyMovement.IsGrounded)
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

        while (hitAbove.collider != null && hitAbove.collider.CompareTag(Tags.GROUND_TAG))
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

    void Jump()
    {
        float jumpForce = Mathf.Sqrt(2f * myRigidbody.gravityScale * jumpHeight);
        float unstuckForce = 2f; 
        myRigidbody.AddForce(new Vector2(Mathf.Sign(transform.localScale.x) * unstuckForce, jumpForce), ForceMode2D.Impulse);
    }

}
