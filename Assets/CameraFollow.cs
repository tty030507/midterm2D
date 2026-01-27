using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private PlayerController playerCtrl;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerCtrl = p.GetComponent<PlayerController>();
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;

            if (playerCtrl != null)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, -playerCtrl.mapHalfWidth, playerCtrl.mapHalfWidth);
                targetPosition.y = Mathf.Clamp(targetPosition.y, -playerCtrl.mapHalfHeight, playerCtrl.mapHalfHeight);
            }

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
    }
}