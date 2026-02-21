using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour {
    // 这是一个单例模式，方便其他脚本调用
    public static GameFlowManager Instance;

    void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // 通用跳转接口
    public void LoadNextStep() {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(nextSceneIndex);
        } else {
            Debug.Log("游戏通关！");
        }
    }
    public void OnPlayerDeath() {
    // 方案 A：直接重启当前关卡
    RestartLevel();
    
    // 或者 方案 B：显示一个“游戏结束”的 UI 面板（稍后讲解）
    // UIManager.Instance.ShowGameOverScreen();
    }

    public void RestartLevel() {
        // 获取当前激活场景的索引并重新加载
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public void BackToMenu() {
        // 假设你的 Menu 场景是 Build Settings 里的第一个
        SceneManager.LoadScene(0);
    }
}