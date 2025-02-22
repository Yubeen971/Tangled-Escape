using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI resultText;

    private int coinCount = 0;
    private float elapsedTime = 0f;
    private bool isGameRunning = true;

    private const float bestTime = 80f;  // 기준 시간: 1분 20초

    public GameObject resultPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (isGameRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    public void AddCoin()
    {
        coinCount++;
        coinText.text = $"Coins: {coinCount}/16";
    }

    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeText.text = $"Time: {minutes}:{seconds:00}";
    }

    public void EndGame()
    {
        isGameRunning = false;
        resultPanel.SetActive(true);
        int score = CalculateScore();
        string grade = GetGrade(score);
        resultText.text = $"🏆 Final Score: {score}\n🎖 Grade: {grade}";
        resultText.gameObject.SetActive(true);
    }

    int CalculateScore()
    {
        float timePenalty = Mathf.Max(0, elapsedTime - bestTime);
        int baseScore = coinCount * 100;
        int timeScore = Mathf.Max(0, 500 - Mathf.FloorToInt(timePenalty * 5));

        return baseScore + timeScore;
    }

    string GetGrade(int score)
    {
        if (score >= 2000) return "S";
        if (score >= 1800) return "A";
        if (score >= 1500) return "B";
        if (score >= 1200) return "C";
        if (score >= 900) return "D";
        return "F";
    }
}
