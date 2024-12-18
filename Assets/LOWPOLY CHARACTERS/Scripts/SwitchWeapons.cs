using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    public GameObject sword;
    public GameObject pistol;
    private Animator playerAnimator;
    private bool isSwordEquipped = false;
    private bool isPistolEquipped = false;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();

        sword.SetActive(false);
        pistol.SetActive(false);
    }

    public void SwitchWeapon(string weaponType)
    {
        sword.SetActive(false);
        pistol.SetActive(false);
        isSwordEquipped = false;
        isPistolEquipped = false;

        if (weaponType == "Sword")
        {
            sword.SetActive(true);
            isSwordEquipped = true;
            playerAnimator.SetTrigger("EquipSword");
        }
        else if (weaponType == "Pistol")
        {
            pistol.SetActive(true);
            isPistolEquipped = true;
            playerAnimator.SetTrigger("EquipPistol");
        }

        if (isSwordEquipped && isPistolEquipped)
        {
            return;
        }
    }

    public bool IsSwordEquipped() => isSwordEquipped;
    public bool IsPistolEquipped() => isPistolEquipped;
}
