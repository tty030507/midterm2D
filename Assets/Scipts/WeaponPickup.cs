using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponType weaponType;
    public float weaponDuration = 8f;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.SetWeapon(weaponType, gameObject, weaponDuration);
        }
    }
}