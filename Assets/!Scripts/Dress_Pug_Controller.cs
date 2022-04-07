using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dress_Pug_Controller : MonoBehaviour
{
    [Header("Pug Attributes")]
    public bool isRotating;
    private Vector3 titlePos, closetPos;
    private Quaternion titleRot, closetRot;
    private Animator pugAnim;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            RotatePug();
        }
    }

    void OnSliderChanged(float value)
    {
        // How much we've changed
        float delta = value - this.previousValue;
        this.transform.Rotate(Vector3.up * delta * 360);

        // Set our previous value for the next change
        this.previousValue = value;

        // Update display slider
        this.pugDisplaySlider.value = this.pugRotateSlider.value;
    }

    public void ToggleSlider(bool toggle)
    {
        if (!toggle)
        {
            pugRotateSlider.value = 0;
            pugDisplaySlider.value = 0;
        }

        pugRotateSlider.gameObject.SetActive(toggle);
        pugDisplaySlider.gameObject.SetActive(toggle);
    }

    private void RotatePug()
    {
        transform.Rotate(Vector3.up * 40.0f * Time.deltaTime);
    }

    public void PlayEmoteOnPug(int emoteIndex)
    {
        pugAnim.SetInteger("playEmote", emoteIndex);
    }
}
