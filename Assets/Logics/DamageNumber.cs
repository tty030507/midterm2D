using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float destroyTime = 0.8f;

    void Start()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0); 
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}