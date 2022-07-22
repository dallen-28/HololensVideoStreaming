using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdatePosition : MonoBehaviour
{
    float offset = -0.7f;
    Quaternion rotationOffset;
    public Transform virtualDisplay;

    bool lockToggled = false;

    // Start is called before the first frame update
    void Start()
    {
        rotationOffset = Quaternion.Euler(0,0,180);
    }

    // Update is called once per frame
    void Update()
    {
        // ToDO: put this code in callback function that executes only when the virtual Display's transform changes

        Vector3 localPosition = new Vector3(offset,0,0);
        this.transform.position = virtualDisplay.transform.localToWorldMatrix.MultiplyPoint(localPosition);
        this.transform.rotation = virtualDisplay.transform.rotation * rotationOffset;
        //this.transform.rotation = virtualDisplay.transform.rotation;
    }
}
