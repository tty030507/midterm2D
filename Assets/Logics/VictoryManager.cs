using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public GameObject victoryPanel;

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        GameFlowManager.Instance.BackToMenu();
    }
}