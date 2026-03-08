using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp = 100f;
    public float attackPower = 1f;
    public float defensePower = 5f;
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

    [Header("Weapon Sounds")]
    public AudioClip defaultShotClip;
    public AudioClip aerosolShotClip;
    public AudioClip glueShotClip;
    public AudioClip gasShotClip;
    private AudioSource audioSource;

    [Header("Damage & Death Sounds")]
    public AudioClip damageClip;
    public AudioClip deathClip;

    private GameObject currentWeaponObject;
    private float weaponTimer;

    private float shootSoundTimer = 0f;
    public float shootSoundInterval = 0.2f; // минимальный интервал между звуками

    [Header("Level System")]
    public LevelUpManager levelUpManager;
    public UnityEngine.UI.Slider expSlider;
    public TMPro.TextMeshProUGUI levelText;
    public AudioClip levelUpClip;
    public int level = 1;
    public float currentExp = 0;
    public float expToNextLevel = 50;

    void Start()
    {
        currentHp = maxHp;
        sr = GetComponent<SpriteRenderer>(); // 获取渲染组件
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHp);
        }

        if (levelText != null)
        {
            levelText.text = "LVL " + level; // Sets it to LVL 1 immediately
        }

        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp; // Sets bar to 0
        }

        if (bladePrefab != null)
        {
            currentBlade = Instantiate(bladePrefab, transform.position, Quaternion.identity);
        }
        audioSource = GetComponent<AudioSource>();
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
        shootSoundTimer -= Time.deltaTime;
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

        // We no longer need mouse position or direction here
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = b.GetComponent<Bullet>();

        float dynamicDamage = 5f;
        GameObject effect = null;
        AudioClip clipToPlay = null;

        // Keep your weapon switch logic to determine damage and sound
        switch (currentWeapon)
        {
            case WeaponType.Default:
                clipToPlay = defaultShotClip;
                dynamicDamage = 5f;
                break;
            case WeaponType.Aerosol:
                clipToPlay = aerosolShotClip;
                dynamicDamage = 5f;
                effect = aerosolEffect;
                break;
            case WeaponType.GlueFoam:
                clipToPlay = glueShotClip;
                dynamicDamage = 10f;
                effect = glueEffect;
                break;
            case WeaponType.Gas:
                clipToPlay = gasShotClip;
                dynamicDamage = 15f;
                effect = gasEffect;
                break;
        }

        if (clipToPlay != null && shootSoundTimer <= 0f)
        {
            audioSource.PlayOneShot(clipToPlay, 1f);
            shootSoundTimer = shootSoundInterval;
        }

        if (bulletScript != null)
        {
            // Pass Vector3.zero for direction since the bullet handles its own aim
            bulletScript.Setup(Vector3.zero, dynamicDamage);
        }

        if (effect != null)
        {
            SpriteRenderer bulletSprite = b.GetComponent<SpriteRenderer>();
            if (bulletSprite != null) bulletSprite.enabled = false;

            GameObject e = Instantiate(effect, b.transform);
            e.transform.localPosition = Vector3.zero;
            e.transform.localScale = new Vector3(0.95f, 0.25f, 0.3f);
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
        if (damageClip != null)
        {
            audioSource.PlayOneShot(damageClip, 1f);
        }
    }

    void Die()
    {
        Debug.Log("Игрок умер!");

        GameFlowManager.Instance.OnPlayerDeath();

        if (deathClip != null)
        {
            // проигрываем звук в позиции игрока, независимо от объекта
            AudioSource.PlayClipAtPoint(deathClip, transform.position, 1f);
        }

        // теперь можно деактивировать объект
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // The string "EXP" here must match the tag you created in Unity exactly
        if (other.CompareTag("EXP"))
        {
            GainExp(10f);
            Destroy(other.gameObject);
            Debug.Log("Collected EXP! Current: " + currentExp);
        }
    }

    void GainExp(float amount)
    {
        currentExp += amount;

        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp = 0;
        expToNextLevel *= 1.2f;
        currentHp = maxHp;
        if (healthBar != null) healthBar.SetMaxHealth(maxHp);

        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel; // Set new "Full" point
            expSlider.value = 0; // Move handle back to 0
        }
        // Play the level up sound
        if (levelUpClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(levelUpClip, 1f);
        }

        if (levelText != null) levelText.text = "LVL " + level; //
        if (levelUpClip != null) audioSource.PlayOneShot(levelUpClip);

        // Trigger the UI instead of automatic stat gains
        if (levelUpManager != null)
        {
            levelUpManager.ShowUpgradeOptions();
        }
    }
}