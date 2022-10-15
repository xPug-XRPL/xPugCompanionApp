using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Cosmetic_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameControllerScript;
    
    [Header("Cosmetic References")]
    public List<GameObject> headItemList;
    public List<GameObject> faceItemList;
    private GameObject selectedHeadItem, selectedFaceItem, previousHeadItem, previousFaceItem;
    public bool permitFaceItemsOverride;

    public void ChooseRandomCosmetics(CosmeticName hatOverride = CosmeticName.Empty, CosmeticName faceOverride = CosmeticName.Empty)
    {
        if (previousHeadItem != null)
        {
            previousHeadItem.SetActive(false);
        }

        if (previousFaceItem != null)
        {
            previousFaceItem.SetActive(false);
        }

        // Select head item
        if (hatOverride == CosmeticName.Empty)
        {
            selectedHeadItem = headItemList[Random.Range(0, headItemList.Count)];
        }
        else
        {
            foreach (GameObject headItem in headItemList)
            {
                if (headItem.GetComponent<Dress_CosmeticObject_Info>().cosmeticName == hatOverride)
                {
                    selectedHeadItem = headItem;
                }
            }
        }
        previousHeadItem = selectedHeadItem;

        // Select face item
        if (faceOverride == CosmeticName.Empty)
        {
            selectedFaceItem = faceItemList[Random.Range(0, faceItemList.Count)];
        }
        else
        {
            foreach (GameObject faceItem in faceItemList)
            {
                if (faceItem.GetComponent<Dress_CosmeticObject_Info>().cosmeticName == faceOverride)
                {
                    selectedFaceItem = faceItem;
                }
            }
        }
        previousFaceItem = selectedFaceItem;

        // Equip both
        selectedHeadItem.SetActive(true);
        
        // Only equip face items if the player has defeated more than 10 enemies
        if (gameControllerScript.enemysDefeated > 10 || permitFaceItemsOverride)
        {
            selectedFaceItem.SetActive(true);
            permitFaceItemsOverride = false;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedHeadItem.SetActive(false);
            selectedFaceItem.SetActive(false);
            ChooseRandomCosmetics();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            selectedFaceItem.SetActive(false);
            selectedHeadItem.SetActive(false);

            // Select head item
            selectedHeadItem = headItemList[3];

            // Select face item
            selectedFaceItem = faceItemList[3];

            // Equip both
            selectedHeadItem.SetActive(true);
            selectedFaceItem.SetActive(true);
        }
    }
}
