using UnityEngine;

public class Blade : MonoBehaviour
{
    [HideInInspector] public float damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(this.damage);
            }
        }
    }
}