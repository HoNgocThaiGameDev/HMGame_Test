using UnityEngine;
using TMPro;
using TestHM;

public class TimeAttackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeRemainText;
    [SerializeField] private float timeAttackDuration = 60f;

    private float timeRemaining;
    //private bool isTimerRunning;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameManager.GameMode.TimeAttack)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timeRemaining = timeAttackDuration;
        //isTimerRunning = true;
        UpdateUIText(timeRemaining);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.CurrentMode != GameManager.GameMode.TimeAttack)
        {
            if (timeRemainText != null && timeRemainText.gameObject.activeSelf)
            {
                timeRemainText.gameObject.SetActive(false);
            }
            return;
        }

        if (timeRemainText != null && !timeRemainText.gameObject.activeSelf)
        {
            timeRemainText.gameObject.SetActive(true);
            ResetTimer(); 
        }

        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    GameManager.Instance.HandleLose();
                    if (TileManager.Instance != null)
                    {
                        TileManager.Instance.ShowLosePanel();
                    }
                }
                UpdateUIText(timeRemaining);
            }
        }
    }

    private void UpdateUIText(float time)
    {
        if (timeRemainText != null)
        {
            int seconds = Mathf.CeilToInt(time);
            timeRemainText.text = seconds.ToString();
        }
    }
}
