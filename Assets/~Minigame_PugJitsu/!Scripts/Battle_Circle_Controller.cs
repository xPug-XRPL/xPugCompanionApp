using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Circle_Controller : MonoBehaviour
{
    [Header("Object Attributes")]
    public float rotateSpeed;
    public bool canRotate;

    void Start()
    {
        UpdateRotateSpeed(100);
    }

    void Update()
    {
        RotateObject();
    }

    public void RotateObject()
    {
        if (canRotate)
        {
            transform.Rotate(new Vector3(0, 0, rotateSpeed) * Time.deltaTime);
        }
    }

    public void UpdateRotateSpeed(float newSpeed)
    {
        rotateSpeed = newSpeed;
    }
}
