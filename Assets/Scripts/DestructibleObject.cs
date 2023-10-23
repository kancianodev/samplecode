using UnityEngine;

public class DestructibleObject : MonoBehaviour, IInteractable
{
    [Header("Object Properties")]
    [SerializeField] private string objectName = "Object";
    [SerializeField] private DestructibleMaterial objectMaterial;
    [SerializeField] private float maxDurability;
    [SerializeField] private GameObject destroyedVersionPrefab;

    private bool isVisible = false;

    private float durability;
    private void Start()
    {
        durability = maxDurability;
    }

    public DestructibleMaterial ObjectMaterial => objectMaterial;

    public void TakeDamage(float damageAmount)
    {
        durability -= damageAmount;
        if (durability <= 0)
        {
            OnDestruction();
            return;
        }

        if(isVisible)
            UIManager.Instance.ShowObjectInfo(objectName, Mathf.RoundToInt(durability) + "/" + maxDurability);
    }

    private void OnDestruction()
    {
        if (destroyedVersionPrefab != null)
        {
            Instantiate(destroyedVersionPrefab, transform.position, transform.rotation);
        }
        UIManager.Instance.HideObjectInfo();
        Destroy(gameObject);
    }

    public void Interact()
    {

    }

    public void OnHover()
    {
        UIManager.Instance.ShowObjectInfo(objectName, Mathf.RoundToInt(durability) + "/" + maxDurability);
        isVisible = true;
    }

    public void OnStopHover()
    {
        UIManager.Instance.HideObjectInfo();
        isVisible = false;
    }
}