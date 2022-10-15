using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolly_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameScript;
    public Attack_Controller playerScript;

    [Header("Dolly Attributes")]
    public bool cameraDollyMode;
    public int currentCameraNumber, prevCameraNumber;
    public Animator dollyAnimator;
    public float dollyStartDelayMin, dollyStartDelayMax, dollyStartDelay;
    public bool repeatingCheckInvoked;

    public bool _savedTempToggle;
    public int _savedIntCam;

    void Start()
    {
        dollyAnimator = GetComponent<Animator>();
    }

    public void ToggleDollyCamera(bool toggle, int initialStartingCam = 1)
    {
        cameraDollyMode = toggle;

        if (toggle)
        {
            if (playerScript.justAttacked && !repeatingCheckInvoked)
            {
                SetPrevCamNumber();
                currentCameraNumber = initialStartingCam;
                dollyAnimator.SetInteger("cameraNumber", initialStartingCam);
            }
            if (!repeatingCheckInvoked)
            {
                _savedTempToggle = toggle;
                _savedIntCam = initialStartingCam;

                InvokeRepeating("QueuedDolly", 0.0f, 0.1f);
            }
        }
        else
        {
            SetPrevCamNumber();
            currentCameraNumber = 0;
            dollyAnimator.SetInteger("cameraNumber", 0);
            repeatingCheckInvoked = false;
        }
    }

    public void QueuedDolly()
    {
        repeatingCheckInvoked = true;
        Debug.Log("WAITING FOR PLAYER TO ATTACK: " + repeatingCheckInvoked + " PLAYER STATUS: " + playerScript.justAttacked);
        if (playerScript.justAttacked)
        {
            ToggleDollyCamera(true, 2);
            repeatingCheckInvoked = false;
            Debug.Log("GOT A CHECK IN!");
            //CancelInvoke("QueuedDolly");
        }
    }

    public int GetRandomCam()
    {
        int randomNum = Random.Range(1, 4);
        if (randomNum == prevCameraNumber)
        {
            if (randomNum == 4)
            {
                randomNum = 1;
            }
            else
            {
                randomNum += 1;
            }
        }

        return randomNum;
    }

    public void AutoTransitionDolly(int newCamNumber)
    {
        if (cameraDollyMode)
        {
            if (newCamNumber == -1)
            {
                // Do random behavior
                SetPrevCamNumber();
                currentCameraNumber = GetRandomCam();
            }
            else if (newCamNumber == -2)
            {
                int tempCamNum = currentCameraNumber;
                // Do transition +1 behavior
                if (currentCameraNumber < 4)
                {
                    tempCamNum += 1;
                }
                else
                {
                    tempCamNum = 1;
                }
                SetPrevCamNumber();
                currentCameraNumber = tempCamNum;
            }
            else if (newCamNumber == -3)
            {
                int tempCamNum = currentCameraNumber;
                // Do transition -1 behavior
                if (currentCameraNumber > 1)
                {
                    tempCamNum -= 1;
                }
                else
                {
                    tempCamNum = 4;
                }
                SetPrevCamNumber();
                currentCameraNumber = tempCamNum;
            }
            else
            {
                SetPrevCamNumber();
                currentCameraNumber = newCamNumber;
            }
            dollyAnimator.SetInteger("cameraNumber", currentCameraNumber);
        }
        else
        {
            // Disable dolly
            StopCoroutine(gameScript.RandomDollyCameraStart());
            ToggleDollyCamera(false);
            currentCameraNumber = 0;
            dollyAnimator.SetInteger("cameraNumber", 0);
        }
    }

    private void SetPrevCamNumber()
    {
        prevCameraNumber = currentCameraNumber;
    }
}
