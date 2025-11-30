using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestHM
{
    public class HomeScreenManager : MonoBehaviour
    {
        private const string gameSceneName = "GameScene";
        public void LoadGameScene()
        {
            SetAutoplayMode(GameManager.AutoplayMode.None);
            StartNormalGame();
        }

        public void StartAutoPlay()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameMode(GameManager.GameMode.Normal);
            }
            SetAutoplayMode(GameManager.AutoplayMode.Win);
            SceneManager.LoadScene(gameSceneName);
        }

        public void StartAutoLose()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameMode(GameManager.GameMode.Normal);
            }
            SetAutoplayMode(GameManager.AutoplayMode.Lose);
            SceneManager.LoadScene(gameSceneName);
        }

        public void StartTimeAttack()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameMode(GameManager.GameMode.TimeAttack);
            }
            SceneManager.LoadScene(gameSceneName);
        }

        public void StartNormalGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameMode(GameManager.GameMode.Normal);
            }
            SceneManager.LoadScene(gameSceneName);
        }

        private void SetAutoplayMode(GameManager.AutoplayMode mode)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetAutoplayMode(mode);
            }
        }
    }
}
