using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrade_Screen_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameScript;
    public Attack_Controller playerAttackController;

    [Header("Upgrade Attributes")]
    public Animator upgradeAnim;
    public int currentSlotNumber;
    public List<Button> buttonObjects;
    public List<TextMeshProUGUI> buttonTexts;
    public GameObject buttonGroupObject;
    public GameObject selectorObject;
    public Animator selectorAnim;
    private bool pressedButton;

    public Vector3 buttonGroupPosition, buttonGroupPositionLeft, buttonGroupPositionRight;

    private void Start()
    {
        upgradeAnim = GetComponent<Animator>();
        currentSlotNumber = 1;
        buttonGroupObject.transform.localPosition = buttonGroupPosition;
    }

    public void DoShiftLeft()
    {
        if (currentSlotNumber == 1)
        {
            /*upgradeAnim.SetBool("shiftLeft", true);
            upgradeAnim.SetBool("shiftRight", false);*/
            currentSlotNumber = 0;
            buttonGroupObject.transform.localPosition = buttonGroupPositionLeft;
        }
        else if (currentSlotNumber == 2)
        {
            //upgradeAnim.SetBool("shiftRight", false);
            currentSlotNumber = 1;
            buttonGroupObject.transform.localPosition = buttonGroupPosition;
        }

        selectorAnim.SetTrigger("shift");
    }

    public void DoShiftRight()
    {
        if (currentSlotNumber == 1)
        {
            /*upgradeAnim.SetBool("shiftRight", true);
            upgradeAnim.SetBool("shiftLeft", false);*/
            currentSlotNumber = 2;
            buttonGroupObject.transform.localPosition = buttonGroupPositionRight;
        }
        else if (currentSlotNumber == 0)
        {
            //upgradeAnim.SetBool("shiftLeft", false);
            currentSlotNumber = 1;
            buttonGroupObject.transform.localPosition = buttonGroupPosition;
        }

        selectorAnim.SetTrigger("shift");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            DoShiftLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            DoShiftRight();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            //DoButton(currentSlotNumber + 1);
            if (!pressedButton)
            {
                pressedButton = true;
                StartCoroutine(DelayDoButton(1.25f, currentSlotNumber + 1));
            }
        }
    }

    public void DoButton(int buttonNumber)
    {
        if (buttonObjects[buttonNumber - 1].interactable)
        {
            Button tempButton = buttonObjects[buttonNumber - 1];
            switch (buttonNumber)
            {
                case 1:
                    playerAttackController.UpgradeAttack(tempButton);
                    break;
                case 2:
                    playerAttackController.UpgradeShield(tempButton);
                    break;
                case 3:
                    playerAttackController.UpgradeHealth(tempButton);
                    break;
            }
            buttonObjects[buttonNumber-1].GetComponent<Animator>().SetTrigger("select");

            gameScript.ResetRound();
            gameObject.SetActive(false);
            gameScript.ToggleModelExceptWireframe(false);
        }
        else
        {
            Debug.Log("Selected button is not interactable!");
        }
    }

    private IEnumerator DelayDoButton(float delayAmount, int buttonNumber)
    {
        if (buttonObjects[buttonNumber - 1].interactable)
        {
            Button tempButton = buttonObjects[buttonNumber - 1];
            switch (buttonNumber)
            {
                case 1:
                    playerAttackController.UpgradeAttack(tempButton);
                    break;
                case 2:
                    playerAttackController.UpgradeShield(tempButton);
                    break;
                case 3:
                    playerAttackController.UpgradeHealth(tempButton);
                    break;
            }
            buttonObjects[buttonNumber - 1].GetComponent<Animator>().SetTrigger("select");
            selectorAnim.SetTrigger("select");

            yield return new WaitForSeconds(delayAmount);

            gameScript.ResetRound();
            gameObject.SetActive(false);
            gameScript.ToggleModelExceptWireframe(false);
            pressedButton = false;
        }
        else
        {
            Debug.Log("Selected button is not interactable!");
        }
    }

    public void TidyUp()
    {
        buttonGroupObject.transform.localPosition = buttonGroupPosition;
        currentSlotNumber = 1;

        // Disable dolly
        StopCoroutine(gameScript.RandomDollyCameraStart());
        gameScript.dollyScript.ToggleDollyCamera(false);
        gameScript.dollyScript.CancelInvoke("QueuedDolly");
        gameScript.dollyScript.currentCameraNumber = 0;
        gameScript.dollyScript.dollyAnimator.SetInteger("cameraNumber", 0);
    }
}
