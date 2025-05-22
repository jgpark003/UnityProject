using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Jump Settings")]
    public float initialJumpForce = 8f;
    public float jumpDecay = 2f;           // 줄어드는 점프력
    public int maxJumpCount = 4;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.25f;
    public Vector2 groundCheckOffset = new Vector2(0f, -0.6f);

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool jumpRequested;
    private bool isJumping;

    private int jumpCount = 0;
    private float currentJumpForce;

    private int groundDelayFrames = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpForce = initialJumpForce;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        RotateVisual();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        CheckGround();

        // 점프 처리
        if (jumpRequested && currentJumpForce > 0f)
        {
            jumpRequested = false;
            isJumping = true;

            // 살짝 띄워서 충돌 방지
            transform.position += transform.up * 0.05f;

            rb.velocity = transform.up.normalized * currentJumpForce;

            jumpCount++;
            currentJumpForce = Mathf.Max(0f, initialJumpForce - jumpDecay * jumpCount);
        }
        else if (isGrounded && !isJumping && groundDelayFrames <= 0)
        {
            Move();
        }

        // 착지 시 점프 상태 초기화
        if (isGrounded && rb.velocity.magnitude < 0.1f && isJumping)
        {
            isJumping = false;
            jumpCount = 0;
            currentJumpForce = initialJumpForce;

            groundDelayFrames = 2; // 이동 딜레이 프레임 수
        }

        // ✅ 프레임 감소는 맨 마지막에
        if (groundDelayFrames > 0)
            groundDelayFrames--;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void RotateVisual()
    {
        if (moveInput != 0)
        {
            transform.Rotate(0f, 0f, -moveInput * 0.3f);
        }
    }

    void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        isGrounded = hit != null;

        Debug.DrawLine(checkPos + Vector2.left * groundCheckRadius, checkPos + Vector2.right * groundCheckRadius, Color.green);
    }
}
