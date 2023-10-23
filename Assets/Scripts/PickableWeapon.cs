using UnityEngine;

public class PickableWeapon : MonoBehaviour, IInteractable
{
    [Header("Weapon Details")]
    [SerializeField] private string weaponName = "Weapon";
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int reserveAmmo;

    public void SetAmmo(int current, int reserve)
    {
        currentAmmo = current;
        reserveAmmo = reserve;
    }

    public void SetWeaponPrefab(GameObject prefab)
    {
        weaponPrefab = prefab;
    }

    public void Interact()
    {
        PlayerWeaponController playerWeaponController = FindObjectOfType<PlayerWeaponController>();
        if (playerWeaponController != null)
        {
            playerWeaponController.EquipWeapon(weaponPrefab);
        }

        Weapon pickedUpWeapon = playerWeaponController.GetComponentInChildren<Weapon>();
        if (pickedUpWeapon)
        {
            pickedUpWeapon.SetAmmo(currentAmmo, reserveAmmo);
        }

        Destroy(gameObject); 
    }

    public void OnHover()
    {
        UIManager.Instance.ShowObjectInfo(weaponName);
    }

    public void OnStopHover()
    {
        UIManager.Instance.HideObjectInfo();
    }
}
