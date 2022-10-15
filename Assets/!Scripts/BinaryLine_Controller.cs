using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryLine_Controller : MonoBehaviour
{
    [Header("Game References")]
    public Wireframe_Scroll wireframeScrollScript;

    [Header("Line Attributes")]
    public float scrollSpeed;
    public Vector3 spawnPosition, respawnPosition;
    public bool wentBack;
    public float dist, times;

    void Start()
    {
        spawnPosition = GetComponent<Transform>().position;

        // Get distance till turn around
        StartCoroutine(WaitTime(GetDistanceTime()));
    }

    void Update()
    {
        if (!wentBack)
        {
            transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed * wireframeScrollScript.scrollSpeedModifier);
        }
        else
        {
            transform.position = spawnPosition;
            wentBack = false;
            StartCoroutine(WaitTime(GetDistanceTime()));
        }
    }

    private float GetDistanceTime()
    {
        float tempDistance = Vector3.Distance(transform.position, respawnPosition);
        dist = tempDistance;
        float tempTime = tempDistance / (scrollSpeed * wireframeScrollScript.scrollSpeedModifier);
        times = tempTime;

        return tempTime;
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        wentBack = true;
    }
}
