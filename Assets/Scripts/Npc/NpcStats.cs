using UnityEngine;

public class NpcStats : MonoBehaviour
{
    [Header("Devriye Ayarları")]
    public Transform targetPoint;
    public Transform startPoint;
    public float moveSpeed = 2f;
    public float waitTime = 5f;

    [Header("Etkileşim Ayarları")]
    public float interactionRange = 2f;
    public KeyCode interactionKey = KeyCode.E;

    private const string WALK_PARAM = "IsWalk";
    private const string USE_PARAM = "IsUse";

    private Animator anim;
    private Rigidbody2D rb;
    private Transform playerTransform;

    private Vector2 startPosition;
    private bool isWaitingAtPoint = false;
    private float waitTimer = 0f;
    private bool goingToTarget = true;
    private bool isInteracting = false;
    private bool playerInRange = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        startPosition = transform.position;

        if (startPoint == null)
        {
            GameObject startObj = new GameObject(gameObject.name + "_StartPoint");
            startObj.transform.position = transform.position;
            startPoint = startObj.transform;
        }

        FindPlayer();
    }

    void Update()
    {
        CheckPlayerInRange();
        HandleInteraction();

        if (isInteracting)
        {
            StopMovement();
            return;
        }

        HandlePatrol();
    }

    void HandleInteraction()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            isInteracting = !isInteracting;

            if (isInteracting)
            {
                StopMovement();
                if (anim != null)
                    anim.SetTrigger(USE_PARAM);
                FacePlayer();
            }
        }

        if (isInteracting && !playerInRange)
            isInteracting = false;
    }

    void HandlePatrol()
    {
        if (targetPoint == null) return;

        if (isWaitingAtPoint)
        {
            waitTimer -= Time.deltaTime;
            SetWalkAnimation(false);

            if (waitTimer <= 0)
            {
                isWaitingAtPoint = false;
                goingToTarget = !goingToTarget;
                FlipTowardsDestination();
            }
            return;
        }

        Vector2 destination = goingToTarget ? (Vector2)targetPoint.position : (Vector2)startPoint.position;
        float distance = Vector2.Distance(transform.position, destination);

        if (distance < 0.3f)
        {
            isWaitingAtPoint = true;
            waitTimer = waitTime;
            StopMovement();
            return;
        }

        MoveTowards(destination);
    }

    void MoveTowards(Vector2 target)
    {
        float direction = target.x - transform.position.x;
        float moveDir = direction > 0 ? 1f : -1f;

        if (Mathf.Sign(transform.localScale.x) != moveDir)
            Flip();

        if (rb != null)
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        SetWalkAnimation(true);
    }

    void FlipTowardsDestination()
    {
        Vector2 destination = goingToTarget ? (Vector2)targetPoint.position : (Vector2)startPoint.position;
        float direction = destination.x - transform.position.x;
        float targetDir = direction > 0 ? 1f : -1f;

        if (Mathf.Sign(transform.localScale.x) != targetDir)
            Flip();
    }

    void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        SetWalkAnimation(false);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void FacePlayer()
    {
        if (playerTransform == null) return;

        float dir = playerTransform.position.x - transform.position.x;
        float targetDir = dir > 0 ? 1f : -1f;

        if (Mathf.Sign(transform.localScale.x) != targetDir)
            Flip();
    }

    void SetWalkAnimation(bool isWalking)
    {
        if (anim != null)
            anim.SetFloat(WALK_PARAM, isWalking ? 1f : 0f);
    }

    void CheckPlayerInRange()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            playerInRange = false;
            return;
        }

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        playerInRange = dist <= interactionRange;
    }

    void FindPlayer()
    {
        GameObject obj = GameObject.FindWithTag("Player");
        if (obj != null)
            playerTransform = obj.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (targetPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 start = startPoint != null ? startPoint.position : transform.position;
            Gizmos.DrawLine(start, targetPoint.position);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetPoint.position, 0.3f);
        }

        if (startPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(startPoint.position, 0.3f);
        }
    }
}
