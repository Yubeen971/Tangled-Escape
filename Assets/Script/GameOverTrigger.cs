using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트가 Player 태그를 가진 경우
        if (other.CompareTag("Player"))
        {
            Debug.Log("💀 Game Over! 플레이어가 게임 오버 블록과 충돌했습니다.");
            
            // 씬을 리로드하여 게임 재시작
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
