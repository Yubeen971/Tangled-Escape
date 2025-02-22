using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.25f;
    private int segmentLength = 35;
    private float lineWidth = 0.1f;

    public Transform player;  // 플레이어 참조
    private bool isPlayerAttached = false;
    private int playerSegmentIndex = 0;

    void Start()
{
    this.lineRenderer = this.GetComponent<LineRenderer>();

    // 플레이어 자동 할당
    if (player == null)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다! Player 태그를 확인하세요.");
        }
    }

    Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    for (int i = 0; i < segmentLength; i++)
    {
        this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
        ropeStartPoint.y -= ropeSegLen;
    }
}


    void Update()
    {
        this.DrawRope();

        if (Input.GetKeyDown(KeyCode.E)) // 플레이어가 로프를 잡거나 놓기
        {
            if (isPlayerAttached)
            {
                DetachPlayer();
            }
            else
            {
                AttachPlayer();
            }
        }
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        Vector2 forceGravity = new Vector2(0f, -1.5f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }

        if (isPlayerAttached)
        {
            player.position = ropeSegments[playerSegmentIndex].posNow; // 플레이어 위치를 로프 세그먼트에 고정
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.ropeSegments[0] = firstSegment;

        for (int i = 0; i < this.segmentLength - 1; i++)
        {
            RopeSegment firstSeg = this.ropeSegments[i];
            RopeSegment secondSeg = this.ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector2 changeDir = (dist > ropeSegLen) ? (firstSeg.posNow - secondSeg.posNow).normalized : (secondSeg.posNow - firstSeg.posNow).normalized;
            Vector2 changeAmount = changeDir * error;

            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                this.ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                this.ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for (int i = 0; i < this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    private void AttachPlayer()
    {
        float closestDist = float.MaxValue;

        for (int i = 0; i < ropeSegments.Count; i++)
        {
            float dist = Vector2.Distance(player.position, ropeSegments[i].posNow);
            if (dist < closestDist)
            {
                closestDist = dist;
                playerSegmentIndex = i;
            }
        }

        isPlayerAttached = true;
        player.GetComponent<Rigidbody2D>().isKinematic = true; // 중력 제거
    }

    private void DetachPlayer()
    {
        isPlayerAttached = false;
        player.GetComponent<Rigidbody2D>().isKinematic = false; // 중력 다시 활성화
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
    public Transform GetClosestSegment(Vector2 playerPosition)
{
    float closestDist = float.MaxValue;
    Transform closestSegment = null;

    foreach (Transform segment in GetComponentsInChildren<Transform>())
    {
        float dist = Vector2.Distance(playerPosition, segment.position);
        if (dist < closestDist)
        {
            closestDist = dist;
            closestSegment = segment;
        }
    }

    return closestSegment;
}
}