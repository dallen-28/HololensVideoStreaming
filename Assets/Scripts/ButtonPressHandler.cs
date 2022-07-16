using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class ButtonPressHandler : MonoBehaviour
{
    public GameObject virtualDisplay;

    ObjectManipulator objectManipulator;
    BoundsControl boundsControl;
     
    
    bool lockToggled;

    public void Start()
    {
        lockToggled = false;
        objectManipulator = virtualDisplay.GetComponent<ObjectManipulator>();
        boundsControl = virtualDisplay.GetComponent<BoundsControl>();
    }

    public void OnLock()
    {
        Debug.Log("TEST");
        if (!lockToggled)
        {
            objectManipulator.enabled = false;
            boundsControl.enabled = false;
            lockToggled = true;
            Debug.Log("Lock Toggled");

        }
        else
        {
            objectManipulator.enabled = true;
            boundsControl.enabled = true;
            lockToggled = false;
            Debug.Log("Lock UnToggled");
        }

    }
}
