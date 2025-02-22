using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;         // 이동 속도
    public float jumpForce = 7f;         // 점프 힘
    public float wallJumpForce = 10f;    // 벽 점프 힘
    public float wallSlideSpeed = 1.5f;  // 벽 미끄러짐 속도

    public Transform groundCheck;  // 바닥 체크 위치
    public Transform wallCheck;    // 벽 체크 위치
    public LayerMask groundLayer;  // 바닥 레이어
    public LayerMask wallLayer;    // 벽 레이어

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isAttachedToRope;
    private Transform ropeSegment; // 현재 붙어 있는 로프 세그먼트
    private float horizontalMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Z축 회전 고정
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 플레이어가 로프에 붙어있지 않으면 일반 이동 허용
        if (!isAttachedToRope)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal");

            // 바닥 & 벽 체크
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

            // 벽 슬라이딩 감지
            if (isTouchingWall && !isGrounded && horizontalMove != 0)
            {
                isWallSliding = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
            else
            {
                isWallSliding = false;
            }

            // 점프 입력 처리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    Jump();
                }
                else if (isWallSliding) // 벽 점프
                {
                    WallJump();
                }
            }
        }

        // 로프 잡기 및 놓기 (E 키)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isAttachedToRope)
            {
                DetachFromRope();
            }
            else
            {
                TryAttachToRope();
            }
        }
    }

    void FixedUpdate()
    {
        // 벽 점프 중이 아닐 때만 이동 적용
        if (!isWallJumping && !isAttachedToRope)
        {
            rb.linearVelocity = new Vector2(horizontalMove * moveSpeed, rb.linearVelocity.y);
        }

        // 로프에 붙어 있는 동안에는 로프에 따라 움직임
        if (isAttachedToRope && ropeSegment != null)
        {
            rb.position = ropeSegment.position;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void WallJump()
    {
        isWallJumping = true;

        // 벽 점프 방향 결정
        float jumpDirection = -Mathf.Sign(rb.linearVelocity.x);
        rb.linearVelocity = new Vector2(jumpDirection * wallJumpForce, jumpForce);

        // 일정 시간 후 벽 점프 상태 해제 (0.2초 후)
        Invoke(nameof(ResetWallJump), 0.2f);
    }

    void ResetWallJump()
    {
        isWallJumping = false;
    }

    // 로프 잡기 시도
    void TryAttachToRope()
    {
        Rope rope = FindObjectOfType<Rope>(); // 현재 씬에서 로프 찾기
        if (rope != null)
        {
            ropeSegment = rope.GetClosestSegment(transform.position);
            if (ropeSegment != null)
            {
                AttachToRope(ropeSegment);
            }
        }
    }

    // 로프에 매달리기
    void AttachToRope(Transform segment)
    {
        isAttachedToRope = true;
        rb.isKinematic = true;  // 중력 제거
        ropeSegment = segment;
    }

    // 로프에서 내려오기
    void DetachFromRope()
    {
        isAttachedToRope = false;
        rb.isKinematic = false; // 중력 다시 활성화
        ropeSegment = null;
    }
}
