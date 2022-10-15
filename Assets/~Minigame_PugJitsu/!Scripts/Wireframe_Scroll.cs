using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wireframe_Scroll : MonoBehaviour
{
    [Header("Game References")]
    public Game_Controller gameScript;

    [Header("Scroll Attributes")]
    public float scrollX = 0.5f;
    public float scrollY = 0.5f;
    public float scrollSpeedModifier, scrollSpeedIncrement, scrollSpeedIncreasingDuration;

    private void Update()
    {
        float offsetX = Time.time * (scrollX * scrollSpeedModifier);
        float offsetY = Time.time * (scrollY * scrollSpeedModifier);
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    public void ModifyScrollSpeedModifier(float newValue)
    {
        //StartCoroutine(LerpValueOverTime(scrollSpeedModifier, newValue, scrollSpeedIncreasingDuration));
        //StartCoroutine(SmoothLerp(newValue));
        scrollSpeedModifier = newValue;
    }

    // Thanks to [https://stackoverflow.com/questions/49750245/lerp-between-two-values-over-time]
    /*IEnumerator LerpValueOverTime(float fromVal, float toVal, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;

            float t = counter / duration;
            t = t * t * (3f - 2f * t);
            float val = Mathf.Lerp(fromVal, toVal, t);
            scrollSpeedModifier = val;
            yield return null;
        }
    }*/

    // Thanks to [https://answers.unity.com/questions/1501234/smooth-forward-movement-with-a-coroutine.html]
    private IEnumerator SmoothLerp(float targetValue)
    {
        float startVal = scrollSpeedModifier;
        float endVal = targetValue;
        float elapsedTime = 0;

        while (elapsedTime < scrollSpeedIncreasingDuration)
        {
            /*float t = elapsedTime / scrollSpeedIncreasingDuration;
            t = t * t * (3f - 2f * t);*/
            scrollSpeedModifier = Mathf.Lerp(startVal, endVal, elapsedTime / scrollSpeedIncreasingDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
