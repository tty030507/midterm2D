using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;      // 拖入你的主菜单面板
    public GameObject levelSelectionPanel; // 拖入你的选关面板

    // 当点击主菜单的 "Select Level" 按钮时调用
    public void OpenLevelSelection()
    {
        mainMenuPanel.SetActive(false);      // 隐藏主菜单
        levelSelectionPanel.SetActive(true); // 显示选关菜单
    }

    // 当点击选关菜单里的 "Back" 按钮时调用 (可选)
    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    // --- 以下是原本的场景跳转逻辑 ---
    public void StartGame() { SceneManager.LoadScene("01_OpenAnimation"); }
    
    public void Level1() { SceneManager.LoadScene("02_Level1"); } // 确保名称和 Build Profiles 一致
    
    public void Level2() { SceneManager.LoadScene("03_Level2_Boss"); }
    
    public void Level3() { SceneManager.LoadScene("04_Level3_Final"); }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}