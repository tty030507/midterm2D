using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject[] weaponPrefabs;

    public float mapHalfWidth = 20f;
    public float mapHalfHeight = 15f;

    private GameObject currentWeapon;

    void Update()
    {
        if (currentWeapon == null)
        {
            SpawnWeapon();
        }
    }

    void SpawnWeapon()
    {
        int index = Random.Range(0, weaponPrefabs.Length);

        Vector3 pos = new Vector3(
            Random.Range(-mapHalfWidth, mapHalfWidth),
            Random.Range(-mapHalfHeight, mapHalfHeight),
            0
        );

        currentWeapon = Instantiate(weaponPrefabs[index], pos, Quaternion.identity);
    }
}