using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dress_Pug_Controller : MonoBehaviour
{
    [Header("Pug Attributes")]
    public bool emoteSleep;
    private Vector3 titlePos, closetPos;
    private Quaternion titleRot, closetRot;
    private Animator pugAnim;

    [Header("Pug Expression References")]
    public Material pugMaterial;
    public Texture currentPugBodyTexture, prevPugBodyTexture;
    public List<Texture> pugBodyTextures, pugExpressionTextures;

    [Header("Object References")]
    public Slider pugRotateSlider;
    public Slider pugDisplaySlider;
    private float previousValue;

    void Awake()
    {
        // Help thanks to TreyH [https://answers.unity.com/questions/1477968/rotate-gameobject-with-gui-slider.html]
        // Assign a callback for when this slider changes
        this.pugRotateSlider.onValueChanged.AddListener(this.OnSliderChanged);

        // And current value
        this.previousValue = this.pugRotateSlider.value;
    }

    // Start is called before the first frame update
    void Start()
    {
        closetPos = new Vector3(3.513611f, -3.333755f, -3.22898f);
        closetRot = new Quaternion(0.0f, 219.33f, 0.0f, 0.0f);

        pugAnim = GetComponent<Animator>();

        pugRotateSlider.value = 0.3963898f;
        pugDisplaySlider.value = 0.3963898f;

        // Get current pug texture
        currentPugBodyTexture = pugMaterial.mainTexture;
        prevPugBodyTexture = currentPugBodyTexture;
    }

    void OnSliderChanged(float value)
    {
        // How much we've changed
        float delta = value - this.previousValue;
        this.transform.Rotate(-Vector3.up * delta * 360);

        // Set our previous value for the next change
        this.previousValue = value;

        // Update display slider
        this.pugDisplaySlider.value = this.pugRotateSlider.value;
    }

    public void ToggleSlider(bool toggle)
    {
        if (!toggle)
        {
            pugRotateSlider.value = 0.3963898f;
            pugDisplaySlider.value = 0.3963898f;
        }

        pugRotateSlider.gameObject.SetActive(toggle);
        pugDisplaySlider.gameObject.SetActive(toggle);
    }

    public void PlayEmoteOnPug(int emoteIndex)
    {
        pugAnim.SetInteger("playEmote", emoteIndex);

        switch(emoteIndex)
        {
            case 1:
                emoteSleep = true;
                ChangePugExpression("Closed");
                break;
            default:
                emoteSleep = false;
                ChangePugExpression("Neutral");
                break;
        }
    }

    public void ChangePugExpression(string expressionName)
    {
        // Looks for expression on pug's currently equipped skin
        if (expressionName != "Neutral" && expressionName != null)
        {
            foreach (Texture expression in pugExpressionTextures)
            {
                if (expression.name == (currentPugBodyTexture.name + "_" + expressionName))
                {
                    // Save prev expression
                    prevPugBodyTexture = currentPugBodyTexture;

                    currentPugBodyTexture = expression;
                    pugMaterial.mainTexture = currentPugBodyTexture;
                }
            }
        }
        else
        {
            // Set back to neutral
            currentPugBodyTexture = prevPugBodyTexture;
            pugMaterial.mainTexture = currentPugBodyTexture;
        }
    }
}
