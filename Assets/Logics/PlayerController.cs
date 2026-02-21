using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp = 100f;
    private float attackPower = 50f;
    private float defensePower = 5f;
    public float moveSpeed = 5f;

    [Header("References")]
    public GameObject bulletPrefab;
    public GameObject bladePrefab;
    

    [Header("Map Settings")]
    public float mapHalfWidth = 20f;
    public float mapHalfHeight = 15f;

    [Header("Blade")]
    public float bladeOrbitSpeed = 180f;
    public float bladeRadius = 1.5f;
    private GameObject currentBlade;
    private float currentAngle;

    private float fireTimer;
    private bool isDead = false;

    void Start()
    {
        currentHp = maxHp;


        if (bladePrefab != null)
        {
            currentBlade = Instantiate(bladePrefab, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (isDead) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, -mapHalfWidth, mapHalfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, -mapHalfHeight, mapHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, 0);

        fireTimer += Time.deltaTime;
        if (fireTimer >= 0.5f)
        {
            Shoot();
            fireTimer = 0;
        }

        if (currentBlade != null)
        {
            float dynamicDamage = 10f + (attackPower * 0.5f);
            Blade bladeScript = currentBlade.GetComponent<Blade>();
            if (bladeScript != null) bladeScript.damage = dynamicDamage;

            currentAngle += bladeOrbitSpeed * Time.deltaTime;
            float radian = currentAngle * Mathf.Deg2Rad;
            float bladeX = transform.position.x + Mathf.Cos(radian) * bladeRadius;
            float bladeY = transform.position.y + Mathf.Sin(radian) * bladeRadius;
            currentBlade.transform.position = new Vector3(bladeX, bladeY, 0);
            currentBlade.transform.Rotate(0, 0, 360 * Time.deltaTime);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 direction = (mousePos - transform.position).normalized;
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletScript = b.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            float dynamicDamage = 10f + (attackPower * 0.5f);
            bulletScript.Setup(direction, dynamicDamage);
        }
    }

    public void TakeDamage(float damage) {
        currentHp -= damage;
        if (currentHp <= 0) {
            Die();
        }
    }

    void Die() {
        Debug.Log("玩家死亡！");
        // 调用我们下面要写的全局管理逻辑
        GameFlowManager.Instance.OnPlayerDeath();
        
        // 禁用玩家控制或播放死亡动画
        gameObject.SetActive(false); 
    }
}