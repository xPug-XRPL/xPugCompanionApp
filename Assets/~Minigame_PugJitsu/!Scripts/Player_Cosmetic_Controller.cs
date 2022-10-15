using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Cosmetic_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Persistent_Game_Controller persistentControllerScript;

    [Header("Cosmetic References")]
    public List<GameObject> headItemList;
    public List<GameObject> faceItemList;
    private GameObject selectedHeadItem, selectedFaceItem;

    private void Start()
    {
        try
        {
            // Try to equip from persistent
            persistentControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Persistent_Game_Controller>();
            EquipCosmeticFromPersistent();
        }
        catch
        {
            Debug.Log("Could not equip items from persistent!");
        }
    }

    public void EquipCosmeticFromPersistent()
    {
        // UNTIL CHANGES OCCUR (TXT FILE), This list (headItemList and faceItemList) needs to always be matching the Dress list
        Persistent_Game_Controller tempScript = persistentControllerScript;

        int persistentHatIndex = tempScript.eCosmeticHat_SelectedIndex;
        int persistentFaceIndex = tempScript.eCosmeticFace_SelectedIndex;

        if (persistentHatIndex != 0 || persistentFaceIndex != 0)
        {
            // Find name of currently equipped persistent items
            CosmeticName hatNameToEquip = tempScript.PlayersCosmeticEquipped.cosmeticInventoryCompact[0].name;
            CosmeticName faceNameToEquip = tempScript.PlayersCosmeticEquipped.cosmeticInventoryCompact[1].name;

            // Look in headlist and facelist for that item and enable it
            foreach (GameObject hatObject in headItemList)
            {
                if (hatObject.GetComponent<Dress_CosmeticObject_Info>().cosmeticName == hatNameToEquip)
                {
                    selectedHeadItem = hatObject;
                }
            }

            foreach (GameObject faceObject in faceItemList)
            {
                if (faceObject.GetComponent<Dress_CosmeticObject_Info>().cosmeticName == faceNameToEquip)
                {
                    selectedFaceItem = faceObject;
                }
            }

            // Equip both
            selectedHeadItem.SetActive(true);
            selectedFaceItem.SetActive(true);
        }
        else
        {
            Debug.Log("Persistent has no items equipped!");
        }
    }
}
