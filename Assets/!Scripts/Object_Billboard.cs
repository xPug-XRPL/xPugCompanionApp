using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Billboard : MonoBehaviour
{
    public GameObject objectToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(objectToLookAt.transform, Vector3.forward);
    }
}
