using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dress_Slot_Controller : MonoBehaviour
{
    [Header("Slot References")]
    public GameObject rowPrefab;
    public Dress_Cosmetic_Master cosmeticMasterScript;
    private Persistent_Game_Controller persistentGameControllerScript;
    private CosmeticInventory _DressLocalCosmeticInventory;
    private RectTransform rectTransform;
    private Vector2 startContentSize;

    [Header("Slot Overview")]
    public CosmeticType selectedSlotType;
    public GameObject selectedSlot;
    public GameObject prevSelectedSlot;
    private float slotStartingVerticalCoordinate, slotVerticalSpacing, contentAddedHeight, rowStartingHorizontalCoordinate;

    [Header("Slot Memory")]
    public int tabHat_SelectedIndex;
    public int tabFace_SelectedIndex, tabBody_SelectedIndex, tabLeg_SelectedIndex, tabTail_SelectedIndex;
    public List<GameObject> tempSlotTypeList;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slotStartingVerticalCoordinate = -158;
        slotVerticalSpacing = -288;
        contentAddedHeight = 288;
        rowStartingHorizontalCoordinate = 0f;
        persistentGameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Persistent_Game_Controller>();

        // Get indexes to mirror last saved outfit with data stored in persistent
        tabHat_SelectedIndex = persistentGameControllerScript.eCosmeticHat_SelectedIndex;
        tabFace_SelectedIndex = persistentGameControllerScript.eCosmeticFace_SelectedIndex;
        tabBody_SelectedIndex = persistentGameControllerScript.eCosmeticBody_SelectedIndex;
        tabLeg_SelectedIndex = persistentGameControllerScript.eCosmeticLeg_SelectedIndex;
        tabTail_SelectedIndex = persistentGameControllerScript.eCosmeticTail_SelectedIndex;


        _DressLocalCosmeticInventory = persistentGameControllerScript.GetCosmeticInventory();
        startContentSize = rectTransform.sizeDelta;
    }

    void Start()
    {
        PopulateRows(CosmeticType.Hat); // LEFT OFF PROBLEM HERE, RESETS TO 0 CANT GO TO LAST SAVED SLOT
    }

    public void SelectSlot(GameObject newSlot)
    {
        if (selectedSlot != null)
        {
            prevSelectedSlot = selectedSlot;
            Dress_Slot prevSelectedDressSlotScript = prevSelectedSlot.GetComponent<Dress_Slot>();
            prevSelectedDressSlotScript.OnSelection(false);
 
            // Unequip previous cosmetic if it is same type as new [or empty, add tab system]
            if (prevSelectedDressSlotScript.slotStoredCosmeticData.type == newSlot.GetComponent<Dress_Slot>().slotStoredCosmeticData.type || newSlot.GetComponent<Dress_Slot>().slotStoredCosmeticData.type == CosmeticType.Empty)
            {
                cosmeticMasterScript.EquipCosmetic(prevSelectedDressSlotScript.slotStoredCosmeticData.name, false);
                prevSelectedDressSlotScript.isEquipped = false;
            }
        }

        selectedSlot = newSlot;

        // Get index of selected slot in rows
        UpdateSelectedTypeIndex(selectedSlot.GetComponent<Dress_Slot>().indexInRows);
    }

    public void TellCosmeticMasterToEquip(bool toggle)
    {
        if (selectedSlot)
        {
            Dress_Slot selectedSlotScript = selectedSlot.GetComponent<Dress_Slot>();

            // Send message to Hat_Master to tell it what hat to equip
            cosmeticMasterScript.EquipCosmetic(selectedSlotScript.slotStoredCosmeticData.name, toggle);
        }
    }

    public void UpdateSelectedTypeIndex(int newIndex)
    {
        switch (selectedSlotType)
        {
            case CosmeticType.Hat:
                tabHat_SelectedIndex = newIndex;
                break;
            case CosmeticType.Face:
                tabFace_SelectedIndex = newIndex;
                break;
            case CosmeticType.Body:
                tabBody_SelectedIndex = newIndex;
                break;
            case CosmeticType.Leg:
                tabLeg_SelectedIndex = newIndex;
                break;
            case CosmeticType.Tail:
                tabTail_SelectedIndex = newIndex;
                break;
        }
    }

    private int GetSelectedTypeIndex(CosmeticType typeToLookFor)
    {
        int returnInt = 0;

        switch (typeToLookFor)
        {
            case CosmeticType.Hat:
                returnInt = tabHat_SelectedIndex;
                break;
            case CosmeticType.Face:
                returnInt = tabFace_SelectedIndex;
                break;
            case CosmeticType.Body:
                returnInt = tabBody_SelectedIndex;
                break;
            case CosmeticType.Leg:
                returnInt = tabLeg_SelectedIndex;
                break;
            case CosmeticType.Tail:
                returnInt = tabTail_SelectedIndex;
                break;
        }

        return returnInt;
    }

    public void PopulateRows(CosmeticType typeToPopulate)
    {
        // Use copied inventory list to use locally
        // Make list of specitic type
        tempSlotTypeList.Clear();
        
        List<CosmeticData> typeList = new List<CosmeticData>();
        foreach (CosmeticData cosmetic in _DressLocalCosmeticInventory.cosmeticInventory)
        {
            if (cosmetic.type == typeToPopulate)
            {
                // Add empty before first one
                if (typeList.Count == 0)
                {
                    CosmeticData emptyData = new CosmeticData();
                    emptyData.type = CosmeticType.Empty;
                    typeList.Add(new CosmeticData());
                }

                typeList.Add(cosmetic);
            }
        }

        // Get total count of inventory items
        int totalCosmeticsInInventory = typeList.Count;

        // If typelist has no matches, end it here
        if (totalCosmeticsInInventory > 0)
        {
            selectedSlotType = typeToPopulate;

            // Clear out rows if they already are there
            if (transform.childCount > 0)
            {
                foreach (Transform childRow in transform)
                {
                    Destroy(childRow.gameObject);
                }
            }

            // Divide by 3 and round up to get number of rows needed
            int neededRows = Mathf.CeilToInt((float)totalCosmeticsInInventory / 3);

            // Define int to iterate through CosmeticInventory to populate slots
            int cosmeticIndex = 0;

            // Define int to track slot index
            int slotIndexTracker = 0;

            // Set content size
            rectTransform.sizeDelta = startContentSize;

            // Populate the rows and space them according to row number
            for (int rowNum = 0; rowNum < neededRows; rowNum++)
            {
                GameObject newRow = Instantiate(rowPrefab, transform, false);

                if (rowNum == 0)
                {
                    Vector3 tempPos = newRow.transform.localPosition;
                    tempPos.y = slotStartingVerticalCoordinate;
                    newRow.transform.localPosition = tempPos;
                }
                else if (rowNum < 3)
                {
                    Vector3 tempPos = newRow.transform.localPosition;
                    tempPos.y = (slotVerticalSpacing * rowNum) + slotStartingVerticalCoordinate;
                    newRow.transform.localPosition = tempPos;
                }
                else
                {
                    Vector3 tempPos = newRow.transform.localPosition;
                    tempPos.y = (slotVerticalSpacing * rowNum) + slotStartingVerticalCoordinate;
                    newRow.transform.localPosition = tempPos;

                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + contentAddedHeight);
                }

                // Assign each slot in the row CosmeticData according to the order of items owned in CosmeticInventory
                foreach (Transform slot in newRow.transform)
                {
                    if (cosmeticIndex < typeList.Count)
                    {
                        Dress_Slot tempSlot = slot.GetComponent<Dress_Slot>();

                        // Assign cosmeticInventory values to slot
                        tempSlot.slotStoredCosmeticData = typeList[cosmeticIndex];

                        // Assign icon
                        tempSlot.SetDisplayIcon(persistentGameControllerScript.GetCosmeticIcon(tempSlot.slotStoredCosmeticData.name));

                        // Assign index to slot
                        tempSlot.indexInRows = slotIndexTracker;

                        cosmeticIndex++;
                    }
                    else
                    {
                        Debug.Log("Slot will not be filled!");
                    }

                    // Hide slot if it is empty and not the first slot (make better way of doing this)
                    //
                    if (slot.GetComponent<Dress_Slot>().slotStoredCosmeticData.type == CosmeticType.Empty && cosmeticIndex > 1)
                    {
                        // not the culprit 8/10/22
                        slot.gameObject.SetActive(false);
                    }

                    // Add to slot index counter
                    slotIndexTracker++;

                    // Add to temp list
                    tempSlotTypeList.Add(slot.gameObject);
                }
            }

            // Select slot that should be selected if player had cosmetic equipped before changing tab
            foreach (GameObject tempItem in tempSlotTypeList)
            {
                //Debug.Log("TEMP ITEM NAME " + tempItem.name + " TEMP ITEM INDEX " + tempItem.GetComponent<Dress_Slot>().indexInRows);
                //Debug.Log("INDEX LOOKING FOR: " + GetSelectedTypeIndex(selectedSlotType) + " OF TYPE: " + selectedSlotType);
                //Debug.Log("MATCHES: " + (tempItem.GetComponent<Dress_Slot>().indexInRows == GetSelectedTypeIndex(selectedSlotType)));

                if (tempItem.GetComponent<Dress_Slot>().indexInRows == GetSelectedTypeIndex(selectedSlotType))
                {
                    if (tempItem.GetComponent<Dress_Slot>().indexInRows != 0)
                    {
                        tempItem.GetComponent<Dress_Slot>().OnSelection(true);
                        break;
                    }
                    else
                    {
                        //tempItem.GetComponent<Dress_Slot>().ShowThatSelected(true);
                    }
                }
            }
        }
        else
        {
            Debug.Log("No matches for specified type!");
        }
    }
}
