using UnityEngine;

public class VenomSwamp : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public float slowMultiplier = 0.5f; // Reduces speed by 50%
    public float lifespan = 5f; // How long the puddle stays on the ground

    void Start()
    {
        // Puddles should disappear after a while so they don't lag the game
        Destroy(gameObject, lifespan);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Damage the player over time
                player.TakeDamage(damagePerSecond * Time.deltaTime);
                
                // Slow the player (we will add a logic to PlayerController for this)
            }
        }
    }
}