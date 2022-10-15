using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win_Screen_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameScript;
    private int _enemysDefeated, _enemysDefeatedNeededToUpgrade;

    public void NextRound()
    {
        _enemysDefeated = gameScript.enemysDefeated;
        _enemysDefeatedNeededToUpgrade = gameScript.enemysDefeatedNeededToUpgrade;
        Debug.Log("DEFEATED: " + _enemysDefeated + " NEEDED: " + _enemysDefeatedNeededToUpgrade + "logic: " + (_enemysDefeated % _enemysDefeatedNeededToUpgrade));

        if (!gameScript.playerMaxUpgraded)
        {
            if (_enemysDefeated % _enemysDefeatedNeededToUpgrade != 0)
            {
                gameScript.ResetRound();
                gameObject.SetActive(false);
            }
            else
            {
                gameScript.DisplayUpgradeToolkit();
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameScript.ResetRound();
            gameObject.SetActive(false);
        }
    }

    public void CallIncreaseSpiralSpeed()
    {
        gameScript.IncreaseSpiralSpeed(); // used as workaround for choppy spiral increase
    }
}
