using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp = 100f;
    private float attackPower = 1f;
    private float defensePower = 5f;
    public float moveSpeed = 5f;

    [Header("References")]
    public GameObject bulletPrefab;
    public GameObject bladePrefab;
    public HealthBar healthBar; // 添加对 HealthBar 脚本的引用

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
[Header("Sprites (四方向)")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private SpriteRenderer sr;

    [Header("Weapon")]
    public WeaponType currentWeapon = WeaponType.Default;

    [Header("Weapon Effects")]
    public GameObject aerosolEffect;
    public GameObject glueEffect;
    public GameObject gasEffect;

    [Header("Weapon System")]
    public Transform weaponHoldPoint;

    private GameObject currentWeaponObject;
    private float weaponTimer;

    void Start()
    {
        currentHp = maxHp;
        sr = GetComponent<SpriteRenderer>(); // 获取渲染组件
        if (healthBar != null) 
        {
            healthBar.SetMaxHealth(maxHp);
        }
        if (bladePrefab != null)
        {
            currentBlade = Instantiate(bladePrefab, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        if (isDead) return;
        float progress = LevelController.TimeProgress;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        HandleSpriteFlip(x, y);
        Vector3 moveDir = new Vector3(x, y, 0).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, -mapHalfWidth, mapHalfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, -mapHalfHeight, mapHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, 0);

        float currentFireInterval = Mathf.Lerp(0.5f, 0.1f, progress); 
        fireTimer += Time.deltaTime;
        if (fireTimer >= currentFireInterval)
        {
            Shoot();
            fireTimer = 0;
        }

        if (currentBlade != null)
    {
        // 初始速度 180，最快增加到 720 (每秒转2圈)
        float dynamicOrbitSpeed = Mathf.Lerp(180f, 720f, progress);
        
        float dynamicDamage = 10f + (attackPower * 0.5f);
        Blade bladeScript = currentBlade.GetComponent<Blade>();
        if (bladeScript != null) bladeScript.damage = dynamicDamage;

        currentAngle += dynamicOrbitSpeed * Time.deltaTime;
        float radian = currentAngle * Mathf.Deg2Rad;
        float bladeX = transform.position.x + Mathf.Cos(radian) * bladeRadius;
        float bladeY = transform.position.y + Mathf.Sin(radian) * bladeRadius;
        currentBlade.transform.position = new Vector3(bladeX, bladeY, 0);
        
        // 刀刃自身的旋转也可以跟着变快
        currentBlade.transform.Rotate(0, 0, dynamicOrbitSpeed * 2 * Time.deltaTime);
    }
        if (currentWeaponObject != null)
        {
            weaponTimer -= Time.deltaTime;

            if (weaponTimer <= 0)
            {
                Destroy(currentWeaponObject);
                currentWeapon = WeaponType.Default;
            }
        }
    }
    void HandleSpriteFlip(float x, float y)
    {
        if (sr == null) return;

        // 优先判断水平移动 (左右)
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > 0) sr.sprite = rightSprite;
            else if (x < 0) sr.sprite = leftSprite;
        }
        // 否则判断垂直移动 (上下)
        else if (Mathf.Abs(y) > 0)
        {
            if (y > 0) sr.sprite = upSprite;
            else if (y < 0) sr.sprite = downSprite;
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

        GameObject effect = null;

        switch (currentWeapon)
        {
            case WeaponType.Aerosol:
                effect = aerosolEffect;
                break;

            case WeaponType.GlueFoam:
                effect = glueEffect;
                break;

            case WeaponType.Gas:
                effect = gasEffect;
                break;
        }

        if (effect != null)
        {
            // отключаем квадратный спрайт
            SpriteRenderer bulletSprite = b.GetComponent<SpriteRenderer>();
            if (bulletSprite != null)
            {
                bulletSprite.enabled = false;
            }

            // создаём визуальный эффект
            GameObject e = Instantiate(effect, b.transform);
            e.transform.localPosition = Vector3.zero;

            // устанавливаем scale по X,Y,Z = 1
            e.transform.localScale = new Vector3(0.8f, 0.3f, 0.3f);
        }
    }

    public void TakeDamage(float damage) 
    {
        currentHp -= damage;

        // 关键：受伤时更新进度条 UI
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHp);
        }

        if (currentHp <= 0) 
        {
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

    public void SetWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void SetWeapon(WeaponType newWeapon, GameObject weaponObj, float duration)
    {
        currentWeapon = newWeapon;

        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
        }

        currentWeaponObject = weaponObj;

        weaponObj.transform.SetParent(weaponHoldPoint);
        weaponObj.transform.localPosition = Vector3.zero;

        weaponTimer = duration;
    }
}