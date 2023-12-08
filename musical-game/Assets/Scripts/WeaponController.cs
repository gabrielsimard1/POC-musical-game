using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] List<WeaponData> ownedWeapons;
    [SerializeField] LaserAmmoUI laserAmmoUI;

    int currentWeaponIndex = 0;

    public void IncrementSelectedWeapon()
    {
        if (currentWeaponIndex < ownedWeapons.Count - 1)
        {
            currentWeaponIndex++;
            laserAmmoUI.SelectedWeaponUpdated();
        }
    }

    public void DecrementSelectedWeapon()
    {
        if (currentWeaponIndex > 0)
        {
            currentWeaponIndex--;
            laserAmmoUI.SelectedWeaponUpdated();
        }
    }

    public List<WeaponData> GetOwnedWeapons()
    {
        return ownedWeapons;
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
}
