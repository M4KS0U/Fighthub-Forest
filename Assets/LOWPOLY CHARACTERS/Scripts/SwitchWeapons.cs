using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    public GameObject sword;
    public GameObject pistol;

    private void Start()
    {
        sword.SetActive(false);
        pistol.SetActive(false);
    }

    public void SwitchWeapon(string weaponType)
    {
        if (weaponType == "Sword")
        {
            sword.SetActive(true);
            pistol.SetActive(false);
        }
        else if (weaponType == "Pistol")
        {
            pistol.SetActive(true);
            sword.SetActive(false);
        }
    }
}