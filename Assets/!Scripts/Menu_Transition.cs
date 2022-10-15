using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Transition : MonoBehaviour
{
    [Header("Menu References")]
    public Menu_Controller menuController;
    public Animator menuAnimator;

    [Header("Menu Attributes")]
    public TransitionType type;
    public enum TransitionType { lines };

    public void DoTransition(int transID)
    {
        // This will do a specific action depending on which transitionID is being played
        // This can be used to change scenes in the middle of a transition, or move stuff the player doesnt want to see for example

        // This object will store different behaviors for different times the gameobject is called, but perform different functions
        // which depend on the transitionID number, which can be called from anywhere when the transition is called to perform
        Debug.Log("Doing transition for: " + gameObject.name + "!" + " Transition ID: " + transID + ".");

        switch (transID)
        {
            case 0:
                Debug.Log("Action 0");
                menuController.ChangeCameraBackgroundColor(menuController.bgColorList[1]);
                break;
            case 1:
                Debug.Log("Action 1");
                GoToScene(1);
                break;
            case 2:
                Debug.Log("Action 2");
                break;
            case 3:
                Debug.Log("Action 3");
                GoToScene(2);
                break;
            default:
                Debug.Log("Action ?");
                break;
        }
    }

    public void SetAnimatorTransitionID(int ID)
    {
        menuAnimator.SetInteger("transitionID", ID);
    }

    private void GoToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
