using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dress_Slot : MonoBehaviour
{
    [Header("Slot Attributes")]
    public Sprite slotUnselected;
    public Sprite slotSelected;
    private Image slotBackground;
    private Dress_Slot_Controller slotController;

    [Header("Slot Variables")]
    public CosmeticData slotStoredCosmeticData;
    public Image slotDisplayIcon;
    public bool isEquipped;
    public int indexInRows;

    /*public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("SUP");
        //Mouse was clicked outside
        ShowThatSelected(false);
    }*/

    private void Awake()
    {
        slotController = GetComponentInParent<Dress_Slot_Controller>();
        slotBackground = GetComponent<Image>();

        /*if (indexInRows == 0)
        {
            OnSelection(true);
        }*/
    }

    public void OnSelection(bool toggle)
    {
        if (toggle)
        {
            // Player can have slot selected (white) and not have it visible on the pug (maybe add checkmark that it is?)
            slotController.SelectSlot(gameObject);
            ShowThatSelected(true);

            if (!isEquipped)
            {
                isEquipped = true;
                slotController.TellCosmeticMasterToEquip(isEquipped);
            }

            // So white can go away when clicking off button but not on another button
            /*EventSystem.current.SetSelectedGameObject(gameObject);
            Debug.Log(EventSystem.current.currentSelectedGameObject.name + " : CURRENT");*/
        }
        // This is unused within this script
        else
        {
            ShowThatSelected(false);
        }
    }

    public void ShowThatSelected(bool toggle)
    {
        if (toggle)
        {
            slotBackground.sprite = slotSelected;
        }
        else
        {
            slotBackground.sprite = slotUnselected;
        }
    }

    public void SetDisplayIcon(Sprite spriteToSet)
    {
        slotDisplayIcon.sprite = spriteToSet;
    }
}
