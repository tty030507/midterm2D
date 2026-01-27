using UnityEngine;
using TMPro;
public class Enemy : MonoBehaviour
{
    [Header("EnemyStats")]
    public float maxHp = 30f;
    public float currentHp;
    public float attackPower = 5f;
    public float defensePower = 2f;
    public float moveSpeed = 2f;

    [Header("UI")]
    public GameObject damagePopupPrefab;

    private Vector3 moveDir;
    private float moveTimer;

    void Start()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            moveDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
            moveTimer = 2f;
        }
        transform.position += moveDir * moveSpeed * Time.deltaTime;
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