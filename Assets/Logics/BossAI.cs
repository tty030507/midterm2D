using UnityEngine;
using System.Collections;

public enum BossState { Chasing, Charging, Dashing, Spitting }

public class BossAI : MonoBehaviour
{
    public BossState currentState = BossState.Chasing;
    private Transform player;
    private Enemy enemyScript;
    private SpriteRenderer sr;

    [Header("Sprites")]
    public Sprite dashLeft, dashRight, spitLeft, spitRight, up, down, left, right;

    [Header("Venom Settings")]
    public GameObject venomSwampPrefab;
    public float spitCooldown = 5f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemyScript = GetComponent<Enemy>();
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            // Logic: Chase for 3s -> Spit Venom -> Chase -> Dash
            yield return ChasePlayer(3f);
            yield return SpitVenom();
            yield return ChasePlayer(2f);
            yield return DashAttack();
        }
    }

    IEnumerator ChasePlayer(float duration)
    {
        currentState = BossState.Chasing;
        float timer = 0;
        while (timer < duration)
        {
            // Use existing Enemy.cs movement or custom logic here
            UpdateSpriteDirection();
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpitVenom()
    {
        currentState = BossState.Spitting;
        sr.sprite = (player.position.x < transform.position.x) ? spitLeft : spitRight;

        // Visual warning shake
        StartCoroutine(ShakeSprite(1.5f, 0.1f));
        yield return new WaitForSeconds(1.5f);

        if (venomSwampPrefab != null)
        {
            // 2. Loop to spit multiple times (e.g., 3 puddles)
            int puddleCount = 3;
            for (int i = 0; i < puddleCount; i++)
            {
                // Calculate a different random offset for each puddle
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(2f, 6f);
                Vector3 spawnPos = player.position + (Vector3)randomOffset;

                Instantiate(venomSwampPrefab, spawnPos, Quaternion.identity);

                // Optional: Small delay between each spit for "machine gun" effect
                yield return new WaitForSeconds(0.2f);
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator DashAttack()
    {
        currentState = BossState.Charging;
        sr.sprite = (player.position.x < transform.position.x) ? dashLeft : dashRight;

        // Shake harder during the 2s dash charge
        StartCoroutine(ShakeSprite(2.0f, 0.15f));
        yield return new WaitForSeconds(2.0f);

        currentState = BossState.Dashing;
        Vector3 dashDir = (player.position - transform.position).normalized;

        float dashTime = 1.0f; // Increase this (e.g., from 0.5f to 1.0f) for a longer dash duration
        float dashMultiplier = 5f; // Increase this (e.g., from 3f to 5f) for a faster dash speed

        while (dashTime > 0)
        {
            // Applying the multiplier to the base moveSpeed from Enemy.cs
            transform.position += dashDir * (enemyScript.moveSpeed * dashMultiplier) * Time.deltaTime;
            dashTime -= Time.deltaTime;
            yield return null;
        }
    }

    void UpdateSpriteDirection()
    {
        Vector3 diff = player.position - transform.position;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) sr.sprite = diff.x > 0 ? right : left;
        else sr.sprite = diff.y > 0 ? up : down;
    }

    IEnumerator ShakeSprite(float duration, float magnitude)
    {
        // 1. Capture the ACTUAL current local position before the shake starts
        Vector3 originalPos = sr.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 2. Calculate the random offset
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            // 3. ADD the offset to the original position
            // Do NOT just use new Vector3(xOffset, yOffset, originalPos.z)
            sr.transform.localPosition = new Vector3(originalPos.x + xOffset, originalPos.y + yOffset, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Reset to the exact position it was in before the shake
        sr.transform.localPosition = originalPos;
    }
}