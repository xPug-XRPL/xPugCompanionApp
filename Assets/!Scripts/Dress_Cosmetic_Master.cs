using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dress_Cosmetic_Master : MonoBehaviour
{
    [Header("Cosmetic References")]
    public CosmeticEquipped cosmeticTypeEquipped;

    public List<CosmeticPair> cosmeticObjectDatabase;
    public Persistent_Game_Controller persistentGameControllerScript;
    public List<GameObject> cosmeticChildrenList;

    public Dress_Slot_Controller dressSlotScript;

    private void Awake()
    {
        persistentGameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Persistent_Game_Controller>();
       
        //--------
        // makee scene do this on load. scene load and persistent game controller has awake function to look for dresscosmetic master script and do tempInv code
        // LEFT OFF
        CosmeticInventory tempInv = persistentGameControllerScript.GetCosmeticInventory();

        // Get all children cosmetics
        foreach (Transform child in transform)
        {
            cosmeticChildrenList.Add(child.gameObject);
        }

        foreach (CosmeticData data in tempInv.cosmeticInventory)
        {
            CosmeticPair tempCosmeticPair = new CosmeticPair();
            tempCosmeticPair.cosmeticName = data.name;
            tempCosmeticPair.cosmeticType = data.type;

            if (tempCosmeticPair.cosmeticName != CosmeticName.Empty)
            {
                // Go through children to find object to assign
                for (int i = 0; i < cosmeticChildrenList.Count; i++)
                {
                    if (cosmeticChildrenList[i].GetComponent<Dress_CosmeticObject_Info>().cosmeticName == tempCosmeticPair.cosmeticName)
                    {
                        tempCosmeticPair.cosmeticObject = cosmeticChildrenList[i];
                    }
                }
            }

            cosmeticObjectDatabase.Add(tempCosmeticPair);
        }

        // Equip equipped cosmetics if the player has any saved
        Debug.Log("LOADING COSMETICS");
        foreach (var data in persistentGameControllerScript.PlayersCosmeticEquipped.cosmeticInventoryCompact)
        {
            Debug.Log("DATA LOG: " + data.name + " : " + data.type);
            if (data.name != CosmeticName.Empty)
            {
                EquipCosmetic(data.name, true);
            }
        }
    }

    public void SaveCosmeticsEquippedToPersistent()
    {
        persistentGameControllerScript.PlayersCosmeticEquipped.cosmeticInventoryCompact.Clear();
        persistentGameControllerScript.AddToPlayersCosmeticEquipped(CosmeticType.Hat, cosmeticTypeEquipped.HatEquipped);
        persistentGameControllerScript.AddToPlayersCosmeticEquipped(CosmeticType.Face, cosmeticTypeEquipped.FaceEquipped);
        persistentGameControllerScript.AddToPlayersCosmeticEquipped(CosmeticType.Body, cosmeticTypeEquipped.BodyEquipped);
        persistentGameControllerScript.AddToPlayersCosmeticEquipped(CosmeticType.Leg, cosmeticTypeEquipped.LegEquipped);
        persistentGameControllerScript.AddToPlayersCosmeticEquipped(CosmeticType.Tail, cosmeticTypeEquipped.TailEquipped);
    }

    public void SaveCosmeticsSelectedIndexToPersistent()
    {
        persistentGameControllerScript.eCosmeticHat_SelectedIndex = dressSlotScript.tabHat_SelectedIndex;
        persistentGameControllerScript.eCosmeticFace_SelectedIndex = dressSlotScript.tabFace_SelectedIndex;
        persistentGameControllerScript.eCosmeticBody_SelectedIndex = dressSlotScript.tabBody_SelectedIndex;
        persistentGameControllerScript.eCosmeticLeg_SelectedIndex = dressSlotScript.tabLeg_SelectedIndex;
        persistentGameControllerScript.eCosmeticTail_SelectedIndex = dressSlotScript.tabTail_SelectedIndex;
        persistentGameControllerScript.hasSavedPlayerData = true;
    }

    public void EquipCosmetic(CosmeticName cosmeticName, bool toggle)
    {
        Debug.Log("COUNT: " + cosmeticObjectDatabase.Count);
        foreach (CosmeticPair cosmetic in cosmeticObjectDatabase)
        {
            //Debug.Log("HEY IM HERE: " + cosmetic.cosmeticObject.name);
            if (cosmetic.cosmeticName == cosmeticName && cosmetic.cosmeticName != CosmeticName.Empty)
            {
                Debug.Log("TRYING TO EQUIP: " + cosmeticName + " OBJECT IS: " + cosmetic.cosmeticObject.name + " AND IS ACTIVE: " + cosmetic.cosmeticObject.activeInHierarchy);
                cosmetic.cosmeticObject.SetActive(toggle);

                // Set cosmetic bool
                SetCosmeticBool(cosmetic.cosmeticName, cosmetic.cosmeticType, toggle);
            }
        }
    }

    private void SetCosmeticBool(CosmeticName cosmeticNameEquipping, CosmeticType cosmeticTypeEquipping, bool toggle)
    {
        switch (cosmeticTypeEquipping)
        {
            case CosmeticType.Hat:
                cosmeticTypeEquipped.Hat = toggle;
                if (toggle)
                {
                    cosmeticTypeEquipped.HatEquipped = cosmeticNameEquipping;
                }
                else
                {
                    cosmeticTypeEquipped.HatEquipped = CosmeticName.Empty;
                }
                break;
            case CosmeticType.Face:
                cosmeticTypeEquipped.Face = toggle;
                if (toggle)
                {
                    cosmeticTypeEquipped.FaceEquipped = cosmeticNameEquipping;
                }
                else
                {
                    cosmeticTypeEquipped.FaceEquipped = CosmeticName.Empty;
                }
                break;
            case CosmeticType.Body:
                cosmeticTypeEquipped.Body = toggle;
                if (toggle)
                {
                    cosmeticTypeEquipped.BodyEquipped = cosmeticNameEquipping;
                }
                else
                {
                    cosmeticTypeEquipped.BodyEquipped = CosmeticName.Empty;
                }
                break;
            case CosmeticType.Leg:
                cosmeticTypeEquipped.Leg = toggle;
                if (toggle)
                {
                    cosmeticTypeEquipped.LegEquipped = cosmeticNameEquipping;
                }
                else
                {
                    cosmeticTypeEquipped.LegEquipped = CosmeticName.Empty;
                }
                break;
            case CosmeticType.Tail:
                cosmeticTypeEquipped.Tail = toggle;
                if (toggle)
                {
                    cosmeticTypeEquipped.TailEquipped = cosmeticNameEquipping;
                }
                else
                {
                    cosmeticTypeEquipped.TailEquipped = CosmeticName.Empty;
                }
                break;
            default:
                cosmeticTypeEquipped.Empty = toggle;
                break;
        }
    }
}

[Serializable]
public class CosmeticPair
{
    public CosmeticName cosmeticName;
    public GameObject cosmeticObject;
    public CosmeticType cosmeticType;
}

[Serializable]
public class CosmeticEquipped
{
    public bool Empty, Hat, Face, Body, Leg, Tail;
    public CosmeticName HatEquipped, FaceEquipped, BodyEquipped, LegEquipped, TailEquipped;
}
