using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dress_Controller : MonoBehaviour
{
    [Header("Dress References")]
    public GameObject editButton;
    public GameObject closetCloseButton, closetOpenButton, closetHomeButton;
    public GameObject emote1Button, emote2Button, emote3Button;
    public Transform tabController;
    public List<GameObject> tabObjects;
    public GameObject pugObject;
    private Dress_Pug_Controller pugObjectScript;

    [Header("Dress Attributes")]
    public GameObject closetObject;
    public Vector3 closetExtendedPosition, closetClosedPosition;
    public bool isClosetOpen;
    public CameraDetails lastOpenedTabDetails;

    [Header("Camera Details Setup")]
    public CameraDetails poserCamDetails;
    public CameraDetails hatCamDetails, faceCamDetails, bodyCamDetails, legCamDetails, tailCamDetails;

    [HideInInspector]public CameraDetails currentCamDetails;
    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        closetExtendedPosition = new Vector3(-263.44f, 0f, 0f);
        closetClosedPosition = new Vector3(-1083.101f, 0f, 0f);

        ChangeCameraFocus(poserCamDetails);

        // Add tab objects from tabController
        foreach (Transform tab in tabController)
        {
            Debug.Log("ADDING: " + tab.gameObject);
            tabObjects.Add(tab.gameObject);
        }

        ToggleTabInteractability(false);

        // Set first tab player sees to be hat
        lastOpenedTabDetails = hatCamDetails;

        pugObjectScript = pugObject.GetComponent<Dress_Pug_Controller>();
    }

    public void ClosetToggle(bool toggle)
    {
        if (toggle)
        {
            closetObject.transform.localPosition = closetExtendedPosition;

            // Change to the last opened tab's correct camera position
            ChangeCameraFocus(lastOpenedTabDetails);

            // Reset pugs animation/rotation to default
            pugObjectScript.PlayEmoteOnPug(0);
        }
        else
        {
            closetObject.transform.localPosition = closetClosedPosition;

            // Set correct pose
            ChangeCameraFocus(poserCamDetails);
        }

        // Stuff here will happen no matter which toggle is called
        ToggleButtons(toggle);
    }

    private void ToggleButtons(bool toggle)
    {
        isClosetOpen = toggle;
        closetCloseButton.SetActive(toggle);
        closetOpenButton.SetActive(!toggle);
        closetHomeButton.SetActive(!toggle);
        emote1Button.SetActive(!toggle);
        emote2Button.SetActive(!toggle);
        emote3Button.SetActive(!toggle);
        ToggleTabInteractability(toggle);
        pugObjectScript.ToggleSlider(!toggle);
    }

    public void ReturnToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void ToggleTabInteractability(bool toggle)
    {
        foreach (GameObject tab in tabObjects)
        {
            tab.GetComponent<Button>().enabled = toggle;
        }
    }

    public void ChangeCameraFocusFromButton(GameObject buttonPressing)
    {
        CosmeticType buttonPressingTabType = buttonPressing.GetComponent<Dress_TabObject_Info>().tabType;

        if (currentCamDetails.cameraFocusType != buttonPressingTabType)
        {
            // Find CameraDetails depending on tab type
            CameraDetails tempCamDetails;

            switch (buttonPressingTabType)
            {
                case CosmeticType.Hat:
                    tempCamDetails = hatCamDetails;
                    break;
                case CosmeticType.Face:
                    tempCamDetails = faceCamDetails;
                    break;
                case CosmeticType.Body:
                    tempCamDetails = bodyCamDetails;
                    break;
                case CosmeticType.Leg:
                    tempCamDetails = legCamDetails;
                    break;
                case CosmeticType.Tail:
                    tempCamDetails = tailCamDetails;
                    break;
                default:
                    tempCamDetails = poserCamDetails;
                    break;
            }

            currentCamDetails = tempCamDetails;
            lastOpenedTabDetails = tempCamDetails;
        }

        // Change camera to match currentCamDetails
        mainCam.transform.localPosition = currentCamDetails.cameraPosition;
        mainCam.transform.rotation = Quaternion.Euler(new Vector3(currentCamDetails.cameraRotation.x, currentCamDetails.cameraRotation.y, currentCamDetails.cameraRotation.z));
    }

    /*private IEnumerator LerpToPosRot()
    {

    }*/

    public void ChangeCameraFocus(CameraDetails newCamDetails)
    {
        currentCamDetails = newCamDetails;

        // Change camera to match currentCamDetails
        mainCam.transform.localPosition = currentCamDetails.cameraPosition;
        mainCam.transform.rotation = Quaternion.Euler(new Vector3(currentCamDetails.cameraRotation.x, currentCamDetails.cameraRotation.y, currentCamDetails.cameraRotation.z));
    }
}

[Serializable]
public class CameraDetails
{
    public CosmeticType cameraFocusType;
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;
}
