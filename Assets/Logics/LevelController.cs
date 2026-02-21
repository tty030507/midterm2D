using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
    public float levelDuration = 180f; // 3分钟
    private float timer;
    
    [Header("Prefabs")]
    public GameObject creepPrefab;
    public GameObject bossPrefab;

    private bool bossSpawned = false;

    void Start() {
        timer = levelDuration;
        // 根据当前场景名称或索引决定生成逻辑
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName==("03_Level2_Boss")) {
            GenerateBoss();
        }

        if (sceneName.Contains("Level1") || sceneName.Contains("Level3")) {
            StartCoroutine(CreepSpawnerRoutine());
        }
    }

    void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            GameFlowManager.Instance.LoadNextStep();
        }
    }

    void GenerateBoss() {
        if (!bossSpawned) {
            Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
            bossSpawned = true;
        }
    }

    IEnumerator CreepSpawnerRoutine() {
        while (timer > 0) {
            GenerateCreep();
            // 越接近结束，生成速度越快
            float spawnInterval = Mathf.Lerp(0.5f, 5f, timer / levelDuration); 
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void GenerateCreep() {
        // 在随机位置生成小怪
        Vector2 randomPos = new Vector2(Random.Range(-10, 10), Random.Range(-5, 5));
        Instantiate(creepPrefab, randomPos, Quaternion.identity);
    }
}