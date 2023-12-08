using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum WeaponType
{
    BlueLaser,
    RedLaser
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/BasicWeapon")]
public class WeaponData : ScriptableObject
{
    [SerializeField] Sprite projectile;
    [SerializeField] WeaponType weaponType;
    [SerializeField] int damage;
    [SerializeField] float range;
    [SerializeField] bool hasInfiniteAmmo;
    [SerializeField] int ammo; // if doesnt have infinite ammo 
    [SerializeField] TextMeshProUGUI ammoText; // not feasible

    public int GetAmmo()
    {
        return ammo;
    }

    public void IncrementAmmo(int add)
    {
        ammo += add;
        ammoText.text = ammo.ToString();
    }

    public void DecrementAmmo()
    {
        if (ammo > 0)
        {
            ammo--;
            ammoText.text = ammo.ToString();
        }
    }

    public bool HasInfiniteAmmo()
    {
        return hasInfiniteAmmo;
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetRange()
    {
        return range;
    }


}
