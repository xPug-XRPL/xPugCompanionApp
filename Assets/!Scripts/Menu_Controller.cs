using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Controller : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    [Header("Hub World Variables")]
    public List<GameObject> hubWorldList;
    [HideInInspector] public List<Vector3> hubWorldPositions;
    [HideInInspector] public List<float> hubWorldSelectionDelays;
    public GameObject selectedHubWorld, prevSelectedHubWorld;
    public int selectedHubWorldIndex, tempIndexOfHubObject;
    [Range(1.0f, 1000.0f)] public float normalRotateSpeed;
    [Range(1.0f, 500.0f)] public float slowRotateSpeed;
    [Range(0.0f, 5.0f)] public float spamCooldown, transitionDelay;
    public float selectionDelay;
    [Range(0.1f, 100.0f)] public float transitionSpeed;
    [Range(0.1f, 50.0f)] public float menuLeaveDistance;
    [Range(0.1f, 100.0f)] public float menuLeaveSpeed;
    private bool isNotSpamming, hubWorldLockedIn;

    [Header("Screen Transition Variables")]
    [Range(0.0f, 5.0f)] public float screenTransitionDelay;
    public GameObject transition_Lines;
    public GameObject selectedTransition;
    public int readTransitionID;

    [Header("Menu Modifers")]
    public List<Color> bgColorList;

    // Start is called before the first frame update
    void Start()
    {
        // Get default color
        bgColorList.Add(mainCamera.backgroundColor);

        UpdateHubPositions();
        selectedHubWorld = hubWorldList[0];
        selectedHubWorldIndex = 0;
        readTransitionID = 0;

        HighlightSelectedHubWorld();
        isNotSpamming = true; // This is to prevent spam

        // Get all selection delays for all hub objests
        for (int i = 0; i < hubWorldList.Count; i++)
        {
            hubWorldSelectionDelays.Add(hubWorldList[i].GetComponentInChildren<Menu_Pug_Controller>().localSelectionDelay);
        }

        selectionDelay = hubWorldSelectionDelays[selectedHubWorldIndex];
    }

    // Update is called once per frame
    void Update()
    {
        NavigateMenu();
    }

    public void NavigateMenu(string navDirection = null)
    {
        if ((Input.GetKey(KeyCode.LeftArrow) || navDirection == "Left" ) && isNotSpamming && !hubWorldLockedIn)
        {
            // Set index to find selected hub world
            if (selectedHubWorldIndex == 0)
            {
                selectedHubWorldIndex = hubWorldList.Count - 1; // 3
            }
            else
            {
                selectedHubWorldIndex -= 1;
            }

            prevSelectedHubWorld = selectedHubWorld;
            SelectHubWorld(hubWorldList[selectedHubWorldIndex]);

            StartCoroutine("DelayTransition", "Left");

            HighlightSelectedHubWorld();
            StartCoroutine("StopSpamCooldown", spamCooldown + transitionDelay);
        }

        if ((Input.GetKey(KeyCode.RightArrow) || navDirection == "Right") && isNotSpamming && !hubWorldLockedIn)
        {
            // Set index to find selected hub world
            if (selectedHubWorldIndex == hubWorldList.Count - 1)
            {
                selectedHubWorldIndex = 0;
            }
            else
            {
                selectedHubWorldIndex += 1;
            }

            prevSelectedHubWorld = selectedHubWorld;
            SelectHubWorld(hubWorldList[selectedHubWorldIndex]);

            StartCoroutine("DelayTransition", "Right");

            HighlightSelectedHubWorld();
            StartCoroutine("StopSpamCooldown", spamCooldown + transitionDelay);
        }

        // Lock in hub world if one is selected
        if ((Input.GetKeyDown(KeyCode.Return) || navDirection == "Middle") && isNotSpamming)
        {
            CheckHubWorldSelection();
        }
    }

    private IEnumerator DelayTransition(string direction)
    {
        yield return new WaitForSeconds(transitionDelay);

        if (direction == "Left")
        {
            // Move hub worlds around
            foreach (GameObject hubObject in hubWorldList)
            {
                Debug.Log("MOVING: " + hubObject + " AT INDEX: " + tempIndexOfHubObject + " HUB WORLD COUNT: " + (hubWorldList.Count - 1));
                if (tempIndexOfHubObject == hubWorldList.Count - 1)
                {
                    hubObject.GetComponentInChildren<Menu_Pug_Controller>().GoToNewPosition(hubWorldPositions[0], transitionSpeed);
                    tempIndexOfHubObject = 0;
                    UpdateHubPositions();
                }
                else
                {
                    UpdateHubPositions();
                    tempIndexOfHubObject += 1;
                    hubObject.GetComponentInChildren<Menu_Pug_Controller>().GoToNewPosition(hubWorldPositions[tempIndexOfHubObject], transitionSpeed);
                }
                Debug.Log("WHAT IS TEMP INDEX: " + tempIndexOfHubObject);
            }
        }

        if (direction == "Right")
        {
            // Move hub worlds around
            foreach (GameObject hubObject in hubWorldList)
            {
                if (hubWorldList.IndexOf(hubObject) > 0)
                {
                    tempIndexOfHubObject -= 1;
                    hubObject.GetComponentInChildren<Menu_Pug_Controller>().GoToNewPosition(hubWorldPositions[hubWorldList.IndexOf(hubObject) - 1], transitionSpeed);
                    UpdateHubPositions();
                    Debug.Log("moving not first objects");
                }
                else
                {
                    UpdateHubPositions();
                    hubObject.GetComponentInChildren<Menu_Pug_Controller>().GoToNewPosition(hubWorldPositions[hubWorldList.Count - 1], transitionSpeed);
                    tempIndexOfHubObject = hubWorldList.Count - 1;
                    Debug.Log("moving first object");
                }
                Debug.Log("WHAT IS TEMP INDEX: " + tempIndexOfHubObject);
            }
        }
    }

    private void UpdateHubPositions()
    {
        if (hubWorldPositions.Count > 0)
        {
            hubWorldPositions.Clear();
        }

        foreach (GameObject hubObject in hubWorldList)
        {
            hubWorldPositions.Add(hubObject.transform.position);
        }
    }

    private void HighlightSelectedHubWorld()
    {
        // If there was no prev selected world, set all hub worlds as if they were not selected besides the one selected
        if (prevSelectedHubWorld == null)
        {
            foreach (GameObject hubWorld in hubWorldList)
            {
                Menu_Pug_Controller tempController = hubWorld.GetComponentInChildren<Menu_Pug_Controller>();
                Canvas tempCanvas = hubWorld.GetComponentInChildren<Canvas>();

                if (hubWorld != selectedHubWorld)
                {
                    SetRotateSpeed(tempController, slowRotateSpeed);
                    SetAnimatorBool(tempController.canvasLogoAnim, "isSelected", false); // Animate circle on UI layer
                    SetCanvasLayer(tempCanvas, 0);
                    ToggleGlowRingOnWorldHub(tempController, false);
                }
                else
                {
                    SetRotateSpeed(tempController, normalRotateSpeed);
                    SetAnimatorBool(tempController.canvasLogoAnim, "isSelected", true); // Animate circle on UI layer
                    SetCanvasLayer(tempCanvas, 1);
                    ToggleGlowRingOnWorldHub(tempController, true);
                }
            }
        }
        // If the player has already had one selected, just modify the previous to turn it off the the new selected
        else
        {
            Menu_Pug_Controller prevSelectedController = prevSelectedHubWorld.GetComponentInChildren<Menu_Pug_Controller>();
            SetRotateSpeed(prevSelectedController, slowRotateSpeed);
            SetAnimatorBool(prevSelectedController.canvasLogoAnim, "isSelected", false); // Animate circle on UI layer
            SetCanvasLayer(prevSelectedHubWorld.GetComponentInChildren<Canvas>(), 0);
            ToggleGlowRingOnWorldHub(prevSelectedController, false);

            Menu_Pug_Controller selectedController = selectedHubWorld.GetComponentInChildren<Menu_Pug_Controller>();
            SetRotateSpeed(selectedController, normalRotateSpeed);
            SetAnimatorBool(selectedController.canvasLogoAnim, "isSelected", true); // Animate circle on UI layer
            SetCanvasLayer(selectedHubWorld.GetComponentInChildren<Canvas>(), 1);
            ToggleGlowRingOnWorldHub(selectedController, true);
        }
    }

    private void CheckHubWorldSelection()
    {
        if (selectedHubWorld)
        {
            if (!hubWorldLockedIn)
            {
                // Lock in the selected hub world
                LockInHubWorld(selectedHubWorld, true);

                if (prevSelectedHubWorld)
                {
                    LockInHubWorld(prevSelectedHubWorld, false);
                }

                hubWorldLockedIn = true;

                // So player cant stop mid animation (make better way to set selectionDelay per gameobject this class is on)
                StartCoroutine("StopSpamCooldown", spamCooldown + selectionDelay);

                // Set each of the non selected worlds to go down off screen
                foreach (GameObject hWorld in hubWorldList)
                {
                    if (hWorld != selectedHubWorld)
                    {
                        hWorld.GetComponentInChildren<Menu_Pug_Controller>().LeaveMenuScreen("down", menuLeaveSpeed, menuLeaveDistance);
                    }
                }

                // Transition screen with delay
                UpdateTransitionID();
                TransitionUI(transition_Lines, screenTransitionDelay);
            }
            else
            {
                // Player can go back when selecting hub world if this is not commented out
                //UndoHubWorldSelection();
            }
        }
    }

    private void UndoHubWorldSelection()
    {
        LockInHubWorld(selectedHubWorld, false);

        hubWorldLockedIn = false;

        float tempTime = 0.0f;

        // Set each of the non selected worlds to come back on screen
        foreach (GameObject hWorld in hubWorldList)
        {
            if (hWorld != selectedHubWorld)
            {
                if (tempTime == 0.0f)
                {
                    tempTime = hWorld.GetComponentInChildren<Menu_Pug_Controller>().positionTime;
                }
                hWorld.GetComponentInChildren<Menu_Pug_Controller>().LeaveMenuScreen("up", menuLeaveSpeed, menuLeaveDistance);
            }
        }

        StartCoroutine("StopSpamCooldown", spamCooldown + tempTime);
    }

    private void UpdateTransitionID()
    {
        readTransitionID = selectedHubWorldIndex;
    }

    private void SelectHubWorld(GameObject hubWorldToSelect)
    {
        selectedHubWorld = hubWorldToSelect;
        selectionDelay = hubWorldSelectionDelays[selectedHubWorldIndex];
    }

    private void LockInHubWorld(GameObject hubWorld, bool toggle)
    {
        hubWorld.GetComponentInChildren<Menu_Pug_Controller>().LockInThisHubWorld(toggle);
    }

    private void SetRotateSpeed(Menu_Pug_Controller menuPugCont, float speed)
    {
        menuPugCont.rotateSpeed = speed;
    }

    private void SetAnimatorBool(Animator animatorToModify, string boolName, bool value)
    {
        animatorToModify.SetBool(boolName, value);
    }

    private void SetCanvasLayer(Canvas canvasToModify, int layerValue)
    {
        canvasToModify.sortingOrder = layerValue;
    }

    private void ToggleGlowRingOnWorldHub(Menu_Pug_Controller menuPugCont, bool toggle)
    {
        menuPugCont.ToggleGlowRing(toggle);
    }

    private IEnumerator StopSpamCooldown(float waitTime)
    {
        isNotSpamming = false;
        yield return new WaitForSeconds(waitTime);
        isNotSpamming = true;
    }

    private void TransitionUI(GameObject transitionObject, float startingDelay = 0.0f)
    {
        selectedTransition = transitionObject;

        if (startingDelay == 0.0f)
        {
            // This will take a gameobject of the transition and tell it to run its code from its own class
            selectedTransition.GetComponent<Menu_Transition>().SetAnimatorTransitionID(readTransitionID);
        }
        else
        {
            StartCoroutine("DelayTransitionUI", startingDelay);
        }
    }

    private IEnumerator DelayTransitionUI(float delayAmount)
    {
        yield return new WaitForSeconds(delayAmount);

        selectedTransition.SetActive(true);
        selectedTransition.GetComponent<Menu_Transition>().SetAnimatorTransitionID(readTransitionID);

        /*
        // Player can cancel if need to at last moment before transition
        if (Input.GetKey(KeyCode.Escape) || (Input.GetKey(KeyCode.Return)))
        {
            UndoHubWorldSelection();
        }
        else
        {
            selectedTransition.SetActive(true);
            selectedTransition.GetComponent<Menu_Transition>().SetAnimatorTransitionID(readTransitionID);
        }*/
    }

    public void ChangeCameraBackgroundColor(Color newColor)
    {
        mainCamera.backgroundColor = newColor;
    }
}
