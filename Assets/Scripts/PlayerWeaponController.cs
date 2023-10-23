using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponHoldPoint; // Miejsce, w którym broñ bêdzie trzymana (np. rêka gracza)

    private GameObject currentWeaponInstance;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (currentWeaponInstance != null)
            currentWeaponInstance.GetComponent<Weapon>().DropWeapon();

        currentWeaponInstance = Instantiate(weaponPrefab, weaponHoldPoint.position, weaponHoldPoint.rotation);
        currentWeaponInstance.transform.SetParent(weaponHoldPoint);
    }
    private bool hasShot = false;
    private bool wasLMBPressed = false;

    private void Update()
    {
        InputManager inputManager = GetComponent<InputManager>();

        if (inputManager != null && currentWeaponInstance != null)
        {
            Weapon weaponScript = currentWeaponInstance.GetComponent<Weapon>();

            if (weaponScript != null)
            {
                if (weaponScript.IsAutomatic && inputManager.LMB)
                {
                    weaponScript.Fire();
                }
                else if (!weaponScript.IsAutomatic && inputManager.LMB && !wasLMBPressed)
                {
                    weaponScript.Fire();
                    hasShot = true;
                }

                if (!inputManager.LMB && hasShot)
                {
                    hasShot = false;
                }

                wasLMBPressed = inputManager.LMB;

                if (inputManager.Drop)
                {
                    weaponScript.DropWeapon();
                    currentWeaponInstance = null;
                }
            }
        }
    }

}