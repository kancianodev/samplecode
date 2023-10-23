using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

	[Header("Main settings")]
    [SerializeField] private GameObject reticle;
    [SerializeField] private TextMeshProUGUI firstHoverText;
    [SerializeField] private TextMeshProUGUI secodHoverText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Showing two line text and hiding reticle
    public void ShowObjectInfo(string firstLine, string secondLine)
    {
        // Aktualizuj elementy UI
        firstHoverText.text = firstLine;
        firstHoverText.gameObject.SetActive(true);
        secodHoverText.text = secondLine;
        secodHoverText.gameObject.SetActive(true);
    }

    //Showing one line text and hiding reticle
    public void ShowObjectInfo(string firstLine)
    {
        reticle.SetActive(false);

        firstHoverText.text = firstLine;
        firstHoverText.gameObject.SetActive(true);
    }

    //Hiding text and showing reticle
    public void HideObjectInfo()
    {
        reticle.SetActive(true);    

        firstHoverText.gameObject.SetActive(false);
        secodHoverText.gameObject.SetActive(false);
    }

    public void UpdateAmmoDisplay(string name, int currentAmmo, int maxAmmo)
	{
        weaponNameText.text = name;
        ammoText.text = currentAmmo + "/" + maxAmmo;
    }        
}
