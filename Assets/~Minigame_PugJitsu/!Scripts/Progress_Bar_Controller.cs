using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress_Bar_Controller : MonoBehaviour
{
    [Header("Progress Bar References")]
    public Transform pointerTransform;
    public int currentTick;
    public List<GameObject> tickObjectList;
    public bool isMovingToTick;

    private void Update()
    {
        MovePointer();
    }

    private void MovePointer()
    {
        if (!isMovingToTick)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentTick + 1 < tickObjectList.Count)
                {
                    Debug.Log("RIGHT");
                    StartCoroutine(MoveToTick(tickObjectList[currentTick + 1].transform.position, 100.0f, 1));
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentTick - 1 >= 0)
                {
                    Debug.Log("LEFT");
                    StartCoroutine(MoveToTick(tickObjectList[currentTick - 1].transform.position, 100.0f, -1));
                }
            }
        }
    }

    private IEnumerator MoveToTick(Vector3 tickPosition, float speed, int tickIncrement)
    {
        isMovingToTick = true;
        var currentPos = pointerTransform.position;
        currentPos.z = 0;

        tickPosition.y = currentPos.y;
        tickPosition.z = 0;
        var distance = Vector3.Distance(currentPos, tickPosition);
        // TODO: make sure speed is always > 0
        var duration = distance / speed;

        var timePassed = 0f;
        while(timePassed < duration)
        {
            // always a factor between 0 and 1
            var factor = timePassed / duration;

            pointerTransform.position = Vector3.Lerp(currentPos, tickPosition, factor);

            // increase timePassed with Mathf.Min to avoid overshooting
            timePassed += Math.Min(Time.deltaTime, duration - timePassed);

            // "Pause" the routine here, render this frame and continue
            // from here in the next frame
            yield return null;
        }

        pointerTransform.position = tickPosition;

        // Something you want to do when moving is done
        currentTick += tickIncrement;
        isMovingToTick = false;
    }
}
