using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dress_Cosmetic_Master : MonoBehaviour
{
    [Header("Cosmetic References")]
    public CosmeticEquipped cosmeticTypeEquipped;

    [HideInInspector] public List<CosmeticPair> cosmeticObjectDatabase;
    private Persistent_Game_Controller persistentGameControllerScript;
    [HideInInspector] public List<GameObject> cosmeticChildrenList;

    private void Awake()
    {
        persistentGameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Persistent_Game_Controller>();
        
        // Get all children cosmetics
        foreach (Transform child in transform)
        {
            cosmeticChildrenList.Add(child.gameObject);
        }
    }

    private void Start()
    {
        CosmeticInventory tempInv = persistentGameControllerScript.GetCosmeticInventory();

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
    }

    public void EquipCosmetic(CosmeticName cosmeticName, bool toggle)
    {
        foreach (CosmeticPair cosmetic in cosmeticObjectDatabase)
        {
            if (cosmetic.cosmeticName == cosmeticName && cosmetic.cosmeticName != CosmeticName.Empty)
            {
                cosmetic.cosmeticObject.SetActive(toggle);

                // Set cosmetic bool
                SetCosmeticBool(cosmetic.cosmeticType, toggle);
            }
        }
    }

    private void SetCosmeticBool(CosmeticType cosmeticTypeEquipping, bool toggle)
    {
        switch (cosmeticTypeEquipping)
        {
            case CosmeticType.Hat:
                cosmeticTypeEquipped.Hat = toggle;
                break;
            case CosmeticType.Face:
                cosmeticTypeEquipped.Face = toggle;
                break;
            case CosmeticType.Body:
                cosmeticTypeEquipped.Body = toggle;
                break;
            case CosmeticType.Leg:
                cosmeticTypeEquipped.Leg = toggle;
                break;
            case CosmeticType.Tail:
                cosmeticTypeEquipped.Tail = toggle;
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
}
