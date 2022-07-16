using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


public class ButtonPressRouter : MonoBehaviour
{
    public GameObject virtualDisplay;
    
    bool lockToggled;

    // Start is called before the first frame update
    void Start()
    {
        lockToggled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnLock()
    {
        ObjectManipulator om = virtualDisplay.GetComponent<ObjectManipulator>();

        if (!lockToggled)
        {
            om.enabled = false;
            lockToggled = true;
            Debug.Log("Lock Toggled");

        }
        else
        {
            om.enabled = true;
            lockToggled = true;
            Debug.Log("Lock UnToggled");
        }

    }
}
