using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform을 연결할 변수
    public float smoothSpeed = 0.125f;  // 카메라가 따라가는 속도
    private Vector3 offset;  // 카메라와 플레이어 간의 초기 거리

    void Start()
    {
        if (player != null)
        {
            // 처음 카메라와 플레이어의 거리를 저장합니다.
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // 플레이어의 x 위치만 추적하고 y, z는 고정
            Vector3 targetPosition = new Vector3(player.position.x + offset.x, transform.position.y, transform.position.z);
            
            // 부드럽게 이동하도록 보간 처리
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}
