using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string name;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float minDamage = 25f;
    [SerializeField] private float maxDamage = 35f;
    [SerializeField] private float bulletForce = 300f;
    [SerializeField] private List<DestructibleMaterial> destructibleMaterials;

    [Header("Effects")]
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Audio Effects")]
    [SerializeField] private AudioClip shootingSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip reloadSound;

    [Header("Shell Casing")]
    [SerializeField] private GameObject shellCasingPrefab;
    [SerializeField] private Transform shellEjectionPoint;

    [Header("Recoil Settings")]
    [SerializeField] private Vector3 recoilTranslation = new Vector3(0f, 0.01f, -0.02f);
    [SerializeField] private Vector3 recoilRotation = new Vector3(-.1f, .05f, .05f);
    [SerializeField] private float recoilSpeed = 8f;
    [SerializeField] private float returnSpeed = 6f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmoInMagazine = 30;
    [SerializeField] private int currentAmmoInMagazine;
    [SerializeField] private int reserveAmmo = 120;
    [SerializeField] private float reloadTime = 2f;

    [Header("Dropping Settings")]
    [SerializeField] private GameObject droppedWeaponPrefab;

    private float lastShotTime;
    private Camera playerCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isReloading = false;

    public bool IsAutomatic => isAutomatic;

    private void Start()
    {
        playerCamera = Camera.main;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        if (currentAmmoInMagazine == 0 && reserveAmmo > maxAmmoInMagazine)
        {
            currentAmmoInMagazine = maxAmmoInMagazine;
            reserveAmmo -= maxAmmoInMagazine;
        }
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        UIManager.Instance.UpdateAmmoDisplay(name, currentAmmoInMagazine, reserveAmmo);
    }

    public bool CanDestroyMaterial(DestructibleMaterial material)
    {
        return destructibleMaterials.Contains(material);
    }

    public void Fire()
    {
        if (isReloading) return;

        if (currentAmmoInMagazine > 0)
        {
            bool didShoot = false;

            if (isAutomatic)
            {
                if (Time.time - lastShotTime >= 1f / fireRate)
                {
                    Shoot();
                    lastShotTime = Time.time;
                    didShoot = true;
                }
            }
            else
            {
                Shoot();
                didShoot = true;
            }

            if (didShoot)
            {
                currentAmmoInMagazine--;
                UpdateAmmoDisplay();

                if (currentAmmoInMagazine <= 0)
                {
                    Reload();
                }
            }
        }
        else
        {
            Reload();
        }
    }


    private void Shoot()
    {
        AudioManager.instance.PlaySFX(shootingSound);

        EjectShellCasing();
        Instantiate(smokePrefab, firePoint.position, firePoint.rotation);

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, bulletForce))
        {

            DestructibleObject destructibleObject = hit.collider.GetComponent<DestructibleObject>();
            if (destructibleObject != null && CanDestroyMaterial(destructibleObject.ObjectMaterial))
            {
                float damage = Random.Range(minDamage, maxDamage);
                destructibleObject.TakeDamage(damage);

                Rigidbody rb = destructibleObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.AddForce(-hit.normal * (bulletForce / 150), ForceMode.Impulse);
                }

                AudioManager.instance.PlaySFX(hitSound);
            }
        }

        StartCoroutine(RecoilEffect());
    }

    private void EjectShellCasing()
    {
        GameObject shellInstance = Instantiate(shellCasingPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);
        Rigidbody shellRb = shellInstance.GetComponent<Rigidbody>();
        Vector3 ejectDirection = shellEjectionPoint.forward + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        shellRb.AddForce(ejectDirection * 2f, ForceMode.Impulse);
        shellRb.AddTorque(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f), ForceMode.Impulse);
    }

    public void Reload()
    {
        if (reserveAmmo > 0 && currentAmmoInMagazine < maxAmmoInMagazine && !isReloading)
        {
            AudioManager.instance.PlaySFX(reloadSound);
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Animacja reloadu - w przysz³oœci.");
        yield return new WaitForSeconds(reloadTime);

        int ammoToLoad = maxAmmoInMagazine - currentAmmoInMagazine;
        int ammoToDeduct = Mathf.Min(ammoToLoad, reserveAmmo);

        currentAmmoInMagazine += ammoToDeduct;
        reserveAmmo -= ammoToDeduct;

        UpdateAmmoDisplay();
        isReloading = false;
    }
    private IEnumerator RecoilEffect()
    {
        Vector3 randomRecoilTranslation = new Vector3(
            Random.Range(-recoilTranslation.x, recoilTranslation.x),
            Random.Range(-recoilTranslation.y, recoilTranslation.y),
            recoilTranslation.z
        );

        Quaternion randomRecoilRotation = Quaternion.Euler(
            originalRotation.eulerAngles + new Vector3(
                Random.Range(-recoilRotation.x, recoilRotation.x),
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z)
            )
        );

        float elapsedRecoilTime = 0f;
        while (elapsedRecoilTime < 1f)
        {
            elapsedRecoilTime += Time.deltaTime * recoilSpeed;

            transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + randomRecoilTranslation, elapsedRecoilTime);
            transform.localRotation = Quaternion.Slerp(originalRotation, randomRecoilRotation, elapsedRecoilTime);

            yield return null;
        }

        float elapsedReturnTime = 0f;
        while (elapsedReturnTime < 1f)
        {
            elapsedReturnTime += Time.deltaTime * returnSpeed;

            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, elapsedReturnTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, elapsedReturnTime);

            yield return null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    public void DropWeapon()
    {
        if (isReloading) return;

        GameObject droppedWeapon = Instantiate(droppedWeaponPrefab, playerCamera.transform.position + playerCamera.transform.forward, Quaternion.identity);

        droppedWeapon.GetComponent<PickableWeapon>().SetAmmo(currentAmmoInMagazine, reserveAmmo);

        UIManager.Instance.UpdateAmmoDisplay("Hand", 1, 1);

        Destroy(gameObject);
    }

    public void SetAmmo(int currentAmmo, int reserveAmmo_)
	{
        currentAmmoInMagazine = currentAmmo;
        reserveAmmo = reserveAmmo_;
        UpdateAmmoDisplay();
	}
}
