using UnityEngine;
using TMPro;
public class Enemy : MonoBehaviour
{
    [Header("EnemyStats")]
    public float maxHp = 30f;
    public float currentHp;
    private float attackPower = 10f;
    public float defensePower = 2f;
    public float moveSpeed = 3f;
    [Header("Sprites")]
    public Sprite leftSprite;  // 在 Inspector 拖入 rat_left.png
    public Sprite rightSprite; // 在 Inspector 拖入 rat_right.png

    public Sprite upSprite;  // 在 Inspector 拖入 rat_left.png
    public Sprite downSprite; // 在 Inspector 拖入 rat_right.png
    private SpriteRenderer spriteRenderer;
    private Transform player;
    [Header("UI")]
    public GameObject damagePopupPrefab;
    private Transform playerTransform; // 存储玩家的位置引用


    void Start()
    {
        currentHp = maxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (GetComponent<BossAI>() != null && GetComponent<BossAI>().currentState != BossState.Chasing)
        {
            return;
        }
        
        if (playerTransform != null)
        {
            Vector3 diff = playerTransform.position - transform.position;

            // 优化：通过比较 X 和 Y 的距离差来决定显示哪个方向，避免逻辑卡死
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                spriteRenderer.sprite = diff.x > 0 ? rightSprite : leftSprite;
            }
            else
            {
                spriteRenderer.sprite = diff.y > 0 ? upSprite : downSprite;
            }

            // 移动逻辑
            Vector3 direction = diff.normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 重要：删掉所有关于 localScale 的代码，防止图片镜像消失！
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        float finalDamage = Mathf.Max(incomingDamage - defensePower, 1f);
        currentHp -= finalDamage;

        Debug.Log(gameObject.name + " Damage Taken: " + finalDamage + "Current Health: " + currentHp);

        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            var textComp = popup.GetComponentInChildren<TMP_Text>();
            if (textComp != null) textComp.text = finalDamage.ToString();
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(this.attackPower);
            }
        }
    }

    void Die()
    {
        Debug.Log("Enemy Killed");
        Destroy(gameObject);
    }
}