using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hp = 15f;
    public float moveSpeed = 2f;
    private Vector3 moveDir;
    private float moveTimer;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        // 这里用了一种简单的方法，只要撞到的是触发器(Trigger)就扣血
        // 之后我们可以用 Tag 来更精确地区分
        if (other.isTrigger)
        {
            TakeDamage(5);
            // 如果是子弹，撞到后销毁
            if (other.GetComponent<Bullet>() != null) Destroy(other.gameObject);
        }
    }

    void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }
}