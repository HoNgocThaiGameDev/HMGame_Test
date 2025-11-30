using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

namespace TestHM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public enum AutoplayMode { None, Win, Lose }

        public enum GameState { Menu, Playing, Paused, GameOver }
        public GameState CurrentState { get; private set; }

        public int Score { get; private set; }
        public int Level { get; private set; }

        public bool IsGameOver => CurrentState == GameState.GameOver;
        public AutoplayMode PendingAutoplayMode { get; private set; } = AutoplayMode.None;

        public enum GameMode { Normal, TimeAttack }
        public GameMode CurrentMode { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            Score = 0;
            Level = 1;
            CurrentState = GameState.Menu;
            PendingAutoplayMode = AutoplayMode.None;
        }

        public void StartGame()
        {
            CurrentState = GameState.Playing;
        }

        public void ResetGame()
        {
            InitializeGame();
        }

        public void SetAutoplayMode(AutoplayMode mode)
        {
            PendingAutoplayMode = mode;
        }

        public void ClearAutoplayMode()
        {
            PendingAutoplayMode = AutoplayMode.None;
        }

        public void SetGameMode(GameMode mode)
        {
            CurrentMode = mode;
        }

        public void HandleWin()
        {
            if (CurrentState == GameState.GameOver)
            {
                return;
            }

            CurrentState = GameState.GameOver;
        }

        public void HandleLose()
        {
            if (CurrentState == GameState.GameOver)
            {
                return;
            }

            CurrentState = GameState.GameOver;
        }
    }
}