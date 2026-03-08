using UnityEngine;

public class EXPGem : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float magnetRange = 5f; // Distance before it starts flying to you
    private Transform player;
    private bool isFollowing = false;

    void Start()
    {
        // Find the player once at the start
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // If player is close enough, start the magnet effect
        if (distance <= magnetRange)
        {
            isFollowing = true;
        }

        if (isFollowing)
        {
            // Move towards the player's current position
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }
}