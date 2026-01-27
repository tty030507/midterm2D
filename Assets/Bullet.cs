using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    public float speed = 10f;
    [HideInInspector] public float damage;

    public void Setup(Vector3 dir, float damageValue)
    {
        direction = dir.normalized;
        this.damage = damageValue;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}