using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int attackPower = 10;
    public int defensePower = 5;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int incomingDamage)
    {
        int damageTaken = Mathf.Max(incomingDamage - defensePower, 1);
        currentHealth -= damageTaken;

        Debug.Log(gameObject.name + " took " + damageTaken + " damage. Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}