using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }
    public GameObject inventoryScreenUI;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isOpen;

    private bool hasObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        isOpen = false;
        hasObject = false;

        PopulateSlotList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            Debug.Log("i is pressed");
            inventoryScreenUI.SetActive(true);
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            isOpen = false;
        }
    }

    private void PopulateSlotList() // osef
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    public void AddToInventory(string itemName)
    {
        if (hasObject)
        {
            Debug.Log("Le joueur poss�de d�j� un objet");
            return;
        }
        else
        {
            
        }
        whatSlotToEquip = FindNextEmptySlot();

        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position,  whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);

        itemList.Add(itemName);
    }

    private GameObject FindNextEmptySlot()
    {
        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
                return slot;
        }

        return new GameObject();
    }


    public bool CheckIsFull()
    {
        int count = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
                count++;
        }

        if (count == slotList.Count)
            return true;
        return false;
    }
}

// Faire en sorte de localiser uniquement l'object que je touche avec ma souris et pas tout ceux dans la zone

// Faire un seul slot dans l'inventaire.
// Faire en sorte que lorsque l'on prend un object il soit dans le slot puis d�s on en prend un autre lacher celui qu'on a et remplacer dans l'inventaire le nouveau