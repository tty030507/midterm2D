using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    [HideInInspector] public float damage;
    private Vector3 moveDirection;

    public void Setup(Vector3 dummyDir, float damageValue)
    {
        this.damage = damageValue;
        
        // Find the target only ONCE at the start
        Transform target = FindNearestTarget();

        if (target != null)
        {
            // Calculate the straight line to that target
            moveDirection = (target.position - transform.position).normalized;
            
            // Rotate the bullet sprite to face that direction
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // If no enemy is found, just shoot in the direction the player is facing
            moveDirection = transform.right; 
        }

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Move in a straight line
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    Transform FindNearestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        float closestDistance = Mathf.Infinity;
        Transform nearest = null;

        // Combine boss and enemies into one check
        if (boss != null)
        {
            float dist = Vector2.Distance(transform.position, boss.transform.position);
            closestDistance = dist;
            nearest = boss.transform;
        }

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boss"))
        {
            Boss boss = other.GetComponent<Boss>();
            if (boss != null) boss.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}