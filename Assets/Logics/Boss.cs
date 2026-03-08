using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHp = 200f; // Bosses have much higher HP than regular enemies
    public float currentHp;
    public float defensePower = 5f;
    public float contactDamage = 20f;

    [Header("UI & Effects")]
    public GameObject damagePopupPrefab;
    [Header("UI Reference")]
    private HealthBar bossHealthBar;

    void Start()
    {
        currentHp = maxHp; //
        GameObject barObj = GameObject.Find("BossHealthBar");
        if (barObj != null)
        {
            barObj.SetActive(true);
            bossHealthBar = barObj.GetComponent<HealthBar>();
            bossHealthBar.SetMaxHealth(maxHp);
        }
    }

    public void TakeDamage(float incomingDamage)
    {
        float finalDamage = Mathf.Max(incomingDamage - defensePower, 1f);
        currentHp -= finalDamage;

        if (bossHealthBar != null)
        {
            bossHealthBar.SetHealth(currentHp);
        }

        // Show damage numbers using your existing prefab logic
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            var textComp = popup.GetComponentInChildren<TMP_Text>();
            if (textComp != null) textComp.text = finalDamage.ToString();
        }

        // if (bossHealthBar != null) bossHealthBar.SetHealth(currentHp);

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
                player.TakeDamage(contactDamage);
            }
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");

        // Find the VictoryManager in the scene
        VictoryManager vm = Object.FindFirstObjectByType<VictoryManager>();
        if (vm != null)
        {
            vm.ShowVictory();
        }
        else
        {
            // Fallback to your existing logic if no UI is found
            GameFlowManager.Instance.LoadNextStep();
        }

        Destroy(gameObject);
    }
}