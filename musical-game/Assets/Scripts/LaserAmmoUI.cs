using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class LaserAmmoUI : MonoBehaviour
{
    [SerializeField] List<WeaponUI> weaponSelect; // should be in same order as weaponController ownedWeapons list
    [SerializeField] WeaponController weaponController;

    int currentWeaponIndex => weaponController.GetCurrentWeaponIndex();
    List<WeaponData> ownedWeapons => weaponController.GetOwnedWeapons();

    float fadedOpacity = .5f;
    float fullOpacity = 1;
    float unselectedTextSize = 75;
    float selectedTextSize = 90;

    void Start()
    {
        InitWeaponUI();
    }

    void InitWeaponUI()
    {
        for (int i = 0; i < weaponSelect.Count; i++)
        {
            if (i == currentWeaponIndex)
            {
                SelectWeapon(weaponSelect[i]);
            }
            else
            {
                UnselectWeapon(weaponSelect[i]);
            }

            if (!weaponSelect[i].isOwned)
            {
                ToggleElementsVisibility(weaponSelect[i], false);
            }
            else if (weaponSelect[i].ammoText != null)
            {
                weaponSelect[i].ammoText.text = ownedWeapons[i].GetAmmo().ToString();
            }
        }
    }

    public void SelectedWeaponUpdated()
    {
        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (i == currentWeaponIndex)
            {
                SelectWeapon(weaponSelect[i]);
            }
            else
            {
                UnselectWeapon(weaponSelect[i]);
            }
        }
    }

    void SelectWeapon(WeaponUI weaponUI)
    {
        weaponUI.selectedSprite.SetActive(true);
        weaponUI.unselectedSprite.SetActive(false);
        if (weaponUI.ammoText != null)
        {
            SetAmmoTextAlpha(weaponUI.ammoText, fullOpacity);
            SetTextSize(weaponUI.ammoText, selectedTextSize);
        }
    }

    void UnselectWeapon(WeaponUI weaponUI)
    {
        weaponUI.selectedSprite.SetActive(false);
        weaponUI.unselectedSprite.SetActive(true);
        if (weaponUI.ammoText != null)
        {
            SetAmmoTextAlpha(weaponUI.ammoText, fadedOpacity);
            SetTextSize(weaponUI.ammoText, unselectedTextSize);
        }
    }

    void ToggleElementsVisibility(WeaponUI weaponUI, bool isVisible)
    {
        weaponUI.selectedSprite.SetActive(isVisible);
        if (!isVisible)
        {
            weaponUI.unselectedSprite.SetActive(isVisible);
        }
        weaponUI.ammoText.enabled = isVisible;
    }

    void SetAmmoTextAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color currentColor = tmp.color;
        currentColor.a = alpha;
        tmp.color = currentColor;
    }

    void SetTextSize(TextMeshProUGUI tmp, float size)
    {
        tmp.fontSize = size;
    }
}

[System.Serializable]
public class WeaponUI
{
    public GameObject selectedSprite;
    public GameObject unselectedSprite;
    public TextMeshProUGUI ammoText;
    public WeaponType weaponType; // might be useless (so far it is)
    public bool isOwned; // = if not owned, should be hidden in ui
}
