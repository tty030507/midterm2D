using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("设置")]
    public float moveSpeed = 5f;
    public GameObject bulletPrefab; // 需要拖入子弹Prefab
    public GameObject bladePrefab;  // 需要拖入飞刀Prefab

    [Header("环绕飞刀")]
    public float bladeOrbitSpeed = 180f; // 旋转速度
    public float bladeRadius = 1.5f;     // 旋转半径
    private GameObject currentBlade;
    private float currentAngle;

    private float fireTimer;

    void Start()
    {
        // 游戏开始生成一把环绕飞刀
        if (bladePrefab != null)
        {
            currentBlade = Instantiate(bladePrefab, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        // --- 1. WASD 移动 ---
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // --- 2. 鼠标瞄准射击 (每0.5秒) ---
        fireTimer += Time.deltaTime;
        if (fireTimer >= 0.5f)
        {
            Shoot();
            fireTimer = 0;
        }

        // --- 3. 更新环绕飞刀位置 ---
        if (currentBlade != null)
        {
            currentAngle += bladeOrbitSpeed * Time.deltaTime; // 增加角度
            // 计算圆周运动坐标 (Cos, Sin)
            float radian = currentAngle * Mathf.Deg2Rad;
            float bladeX = transform.position.x + Mathf.Cos(radian) * bladeRadius;
            float bladeY = transform.position.y + Mathf.Sin(radian) * bladeRadius;
            
            currentBlade.transform.position = new Vector3(bladeX, bladeY, 0);
            // 让飞刀自己也自转一下，满足你的需求
            currentBlade.transform.Rotate(0, 0, 360 * Time.deltaTime);
        }
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 direction = (mousePos - transform.position).normalized;

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Bullet>().Setup(direction);
    }
}