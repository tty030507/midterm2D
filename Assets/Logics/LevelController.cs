using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro; // 必须有这一行
public class LevelController : MonoBehaviour {
    public float levelDuration = 180f; // 3分钟
    public static float TimeProgress; // 0 代表开始，1 代表结束
    private float timer;
    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // 在 Inspector 中拖入你的 TextMeshPro 物体
    [Header("Prefabs")]
    public GameObject creepPrefab;
    public GameObject bossPrefab;
     [Header("Map Settings")]
    public float mapHalfWidth = 20f;
    public float mapHalfHeight = 15f;
    private bool bossSpawned = false;
    public GameObject[] creepPrefabs; // 数组，可以存老鼠、蛇等多个 Prefab
    void Start() {
        timer = levelDuration;
        // 根据当前场景名称或索引决定生成逻辑
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName.Equals("03_Level2_Boss") || sceneName.Contains("Level3")) {
            GenerateBoss();
        }

        if (sceneName.Contains("Level1") || sceneName.Contains("Level3")) {
            StartCoroutine(CreepSpawnerRoutine());
        }
    }

    void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
            TimeProgress = (levelDuration - timer) / levelDuration;
            UpdateTimerUI(); // 每一帧更新时间显示
        } else {
            TimeProgress = 1f;
            timer = 0;
            GameFlowManager.Instance.LoadNextStep(); // 时间到，跳转下一关
        }
    }
    void UpdateTimerUI() {
        if (timerText != null) {
            int minutes = Mathf.FloorToInt(timer / 60); // 计算分钟
            int seconds = Mathf.FloorToInt(timer % 60); // 计算秒数
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // 格式化为 03:00 这种形式
        }
    }
    void GenerateBoss() {
    if (!bossSpawned) {
        // 1. 生成 Boss 实例
        float spawnX = Random.Range(-mapHalfWidth + 1f, mapHalfWidth - 1f);
        float spawnY = Random.Range(-mapHalfHeight + 1f, mapHalfHeight - 1f);
        Vector2 randomPos = new Vector2(spawnX, spawnY);
        GameObject bossObj = Instantiate(bossPrefab, randomPos, Quaternion.identity);
        
        // 2. 配置 Rigidbody2D (确保物理碰撞正常)
        Rigidbody2D rb = bossObj.GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = bossObj.AddComponent<Rigidbody2D>(); // 如果 Prefab 没挂，就动态加上
        }

        // 设置物理属性
        rb.gravityScale = 0f; // 俯视角游戏不需要重力
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 开启连续检测，防止大体型穿模
        rb.freezeRotation = true; // 锁定 Z 轴旋转，防止 Boss 被撞得乱转

        // 3. 动态添加 Enemy.cs (注意：请确认你的脚本名是 Enemy 还是 Boss)
        // 根据你之前的逻辑，基础属性通常挂在 Enemy.cs 上
        if (bossObj.GetComponent<Enemy>() == null) {
            bossObj.AddComponent<Enemy>();
        }

        // 4. 动态添加 BossAI.cs 行为脚本
        if (bossObj.GetComponent<BossAI>() == null) {
            bossObj.AddComponent<BossAI>();
        }

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
    float spawnX = Random.Range(-mapHalfWidth + 1f, mapHalfWidth - 1f);
    float spawnY = Random.Range(-mapHalfHeight + 1f, mapHalfHeight - 1f);
    Vector2 randomPos = new Vector2(spawnX, spawnY);
    int randomIndex = Random.Range(0, creepPrefabs.Length);
    // 1. 生成小怪实例
    GameObject creepObj = Instantiate(creepPrefabs[randomIndex], randomPos, Quaternion.identity);
    
    // 2. 配置 Rigidbody2D
    Rigidbody2D rb = creepObj.GetComponent<Rigidbody2D>();
    if (rb == null) {
        rb = creepObj.AddComponent<Rigidbody2D>(); // 如果 Prefab 没挂，就动态加上
    }

    // 设置物理属性
    rb.gravityScale = 0f; // 2D 俯视角游戏通常不需要重力
    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 设置为连续检测
    rb.freezeRotation = true; // 防止小怪撞墙后转圈圈

    // 3. 动态添加 Enemy.cs
    if (creepObj.GetComponent<Enemy>() == null) {
        creepObj.AddComponent<Enemy>();
    }

    // 4. 动态添加 SimpleAI.cs
    if (creepObj.GetComponent<SimpleAI>() == null) {
        creepObj.AddComponent<SimpleAI>();
    }
}
}