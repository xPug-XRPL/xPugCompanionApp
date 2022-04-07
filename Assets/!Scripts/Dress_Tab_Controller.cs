using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dress_Tab_Controller : MonoBehaviour
{
    [Header("Tab References")]
    public Dress_Slot_Controller dressSlotControllerScript;

    [Header("Tab Attributes")]
    public GameObject currentTab;
    private Dress_TabObject_Info currentTabInfo;

    public void SelectTab(GameObject tabToSelect)
    {
        currentTab = tabToSelect;
        currentTabInfo = currentTab.GetComponent<Dress_TabObject_Info>();

        dressSlotControllerScript.PopulateRows(currentTabInfo.tabType);
    }
}
