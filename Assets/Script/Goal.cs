using UnityEngine;

public class GoalBlock : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.EndGame();
        }
    }
}
