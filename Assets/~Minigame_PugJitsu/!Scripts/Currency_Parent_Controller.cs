using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency_Parent_Controller : MonoBehaviour
{
    //[Header("Game References")]
    private Currency_Master_Controller currencyScript;

    private void Start()
    {
        currencyScript = GetComponentInParent<Currency_Master_Controller>();
    }

    public void CollectSelf()
    {
        currencyScript.AddToCollectedCurrency();
        // temp action
        Destroy(gameObject);
    }
}
