using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요

public class SceneChanger : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    public string targetSceneName; // 이동할 씬의 이름

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 접촉했을 때
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName); // 씬 이동
            }
            else
            {
                Debug.LogWarning("이동할 씬의 이름이 설정되지 않았습니다!");
            }
        }
    }
}
