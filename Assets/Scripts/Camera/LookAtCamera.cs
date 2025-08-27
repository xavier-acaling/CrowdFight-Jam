using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        targetCamera = Camera.main;
    }

   
    public Camera targetCamera;
    public Vector3 offset = new Vector3(0, 2, 0); 
     public Vector3 rotationOffset = Vector3.zero; 
    void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

         // Apply position offset
        Vector3 basePosition = transform.parent ? transform.parent.position : transform.position;
        transform.position = basePosition + offset;

        // Make the text face the camera
        transform.LookAt(transform.position + targetCamera.transform.forward);

        // Apply rotation offset
        transform.Rotate(rotationOffset);
    }
}
