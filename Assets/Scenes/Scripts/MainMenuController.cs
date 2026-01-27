using UnityEngine;
using UnityEngine.SceneManagement; // 必须有这一行才能管理场景切换

public class MainMenuController : MonoBehaviour
{
    // 这个方法必须是 public，否则按钮在 Inspector 里找不到它
    public void StartGame()
    {
        // 这里的 "Level1" 必须和你组员准备好的地图场景文件名完全一致
        SceneManager.LoadScene("Game Level 1"); 
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting..."); // 在编辑器里测试时会看到这条日志
        Application.Quit(); // 仅在导出的游戏程序中有效
    }
}