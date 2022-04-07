using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Pug_Controller : MonoBehaviour
{
    [Header("Menu Pug Attributes")]
    [Range(-1, 1)] public int rotateDirection;
    [Range(0.0f, 10.0f)] public float localSelectionDelay;
    [HideInInspector] public float rotateSpeed, transitionSpeed;
    public SpriteRenderer glowRingRenderer;
    public Sprite glowRingOn, glowRingOff;
    public GameObject glowRingLight;
    private Vector3 posToGoTo;
    private float positionDistance;
    [HideInInspector] public float positionTime;
    private bool doTransition;
    public bool isLockedIn;
    private Animator meshAnim;

    [Header("References")]
    public Animator canvasLogoAnim;

    [Header("Hub World Selection Variables")]
    public int sceneIndex;
    [HideInInspector] public float menuLeaveDistance;
    private Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        meshAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        RotatePug();

        if (doTransition)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, posToGoTo, transitionSpeed * Time.deltaTime);
        }
    }

    private void RotatePug()
    {
        transform.Rotate(rotateDirection * Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void GoToNewPosition(Vector3 newPosition, float transSpeed)
    {
        // Save new position to go to and speed
        prevPosition = transform.parent.position;
        posToGoTo = newPosition;
        transitionSpeed = transSpeed;

        // Calculate distance from current position to new position
        positionDistance = Vector3.Distance(transform.parent.position, posToGoTo);

        // Calculate time to go distance at transSpeed
        positionTime = (positionDistance / transitionSpeed);

        //Debug.Log("POS DISTANCE: " + positionDistance + " TIME: " + positionTime + " TRANSSPEED: " + transitionSpeed + "PARENT POS: " + transform.parent.position + " POSTOGOTO: " + posToGoTo);

        // Start coroutine to move pug
        StartCoroutine("DelayFurtherMovement");
    }

    private IEnumerator DelayFurtherMovement()
    {
        //InvokeRepeating("TransitionPugToNewPosition", 0.0f, 0.01667f);
        doTransition = true;
        yield return new WaitForSeconds(positionTime);
        doTransition = false;
        //CancelInvoke("TransitionPugToNewPosition");

        // For good measure do final snap to position
        transform.parent.position = posToGoTo;
    }

    /*private void TransitionPugToNewPosition()
    {
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, posToGoTo, transitionSpeed * Time.deltaTime);
    }*/

    public void ToggleGlowRing(bool toggle)
    {
        if (toggle)
        {
            glowRingRenderer.sprite = glowRingOn;
        }
        else
        {
            glowRingRenderer.sprite = glowRingOff;
        }

        glowRingLight.SetActive(toggle);
    }

    public void LockInThisHubWorld(bool toggle)
    {
        // Locking in the selection, meaning the animation will play in the menu for being selected
        isLockedIn = toggle;
        meshAnim.SetBool("isLockedIn", toggle);
    }

    public void LeaveMenuScreen(string moveDirection, float speed, float leaveDistance)
    {
        menuLeaveDistance = leaveDistance;
        Vector3 leavingPosition = transform.parent.position;

        switch (moveDirection)
        {
            case "down":
                leavingPosition.y -= menuLeaveDistance;
                break;
            case "up":
                leavingPosition.y += menuLeaveDistance;
                break;
            case "right":
                leavingPosition.x += menuLeaveDistance;
                break;
            case "left":
                leavingPosition.x -= menuLeaveDistance;
                break;
            default:
                leavingPosition.y -= menuLeaveDistance;
                break;
        }

        GoToNewPosition(leavingPosition, speed);
    }
}
