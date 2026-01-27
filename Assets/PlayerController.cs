using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float currentHp = 100f;
    private float attackPower = 50f;
    private float defensePower = 5f;
    public float moveSpeed = 5f;
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

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - defensePower, 1f);
        currentHp -= finalDamage;
        Debug.Log("Player HP: " + currentHp);
        if (currentHp <= 0) Debug.Log("Player Death");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapHalfWidth * 2, mapHalfHeight * 2, 0));
    }
}