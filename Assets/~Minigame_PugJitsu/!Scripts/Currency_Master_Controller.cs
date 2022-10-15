using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency_Master_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameControllerScript;

    [Header("Currency Attributes")]
    public int currencyCollected;


    public void AddToCollectedCurrency()
    {
        currencyCollected++;
    }
}
