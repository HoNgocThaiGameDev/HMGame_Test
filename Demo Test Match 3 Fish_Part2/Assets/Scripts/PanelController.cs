using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PanelController : MonoBehaviour
{
    [SerializeField] private string homeSceneName = "Home";
    [SerializeField] GameObject panelPause;

    public void OnOkButtonClicked()
    {
        Debug.Log("loading home scene");
        if (!string.IsNullOrEmpty(homeSceneName))
        {
            SceneManager.LoadScene(homeSceneName);
        }
    }

    public void OnPausePanel()
    {
        panelPause.SetActive(true);
        if (TestHM.TileManager.Instance != null)
        {
            TestHM.TileManager.Instance.SetTimerPaused(true);
        }
    }

    public void OnCancelPausePanel()
    {
        panelPause.SetActive(false);
        if (TestHM.TileManager.Instance != null)
        {
            TestHM.TileManager.Instance.SetTimerPaused(false);
        }
    }


}
