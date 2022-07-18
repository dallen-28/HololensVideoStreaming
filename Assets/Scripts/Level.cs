using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public abstract class Level
{
    private const float threshold = 0.01f;

    public GameObject nextButton;

    public string titleText;
    public string panelText;

    public Transform startingPoint;
    public Transform currentPoint;
    public Transform targetPoint;
    public int levelNumber;

    public abstract bool CheckForCompletion();

    public abstract void SetManipulationType(GameObject movingPanel);

    public string formattedText()
    {
        return "<size=42><b>" + titleText + "</b></size>\n\n" + panelText;
    }

    // Determines if moving panel's X position is within range of the target position
    protected bool XposInRange()
    {
        if (Mathf.Abs(targetPoint.position.x - currentPoint.position.x) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determines if moving panel's Y position is within range of target position
    protected bool YposInRange()
    {
        if (Mathf.Abs(targetPoint.position.y - currentPoint.position.y) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's Z position is within range of target position
    protected bool ZposInRange()
    {
        if (Mathf.Abs(targetPoint.position.z - currentPoint.position.z) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's X Rotation is within range of target rotation
    protected bool XRotInRange()
    {
        if(Mathf.Abs(targetPoint.rotation.eulerAngles.x - currentPoint.rotation.eulerAngles.x) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's Y Rotation is within range of target rotation
    protected bool YRotInRange()
    {
        if (Mathf.Abs(targetPoint.rotation.eulerAngles.y - currentPoint.rotation.eulerAngles.y) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's Z Rotation is within range of target rotation
    protected bool ZRotInRange()
    {
        if (Mathf.Abs(targetPoint.rotation.eulerAngles.z - currentPoint.rotation.eulerAngles.z) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's scale is within range of target scale
    protected bool ScaleInRange()
    {
        if(Mathf.Abs(targetPoint.localScale.x - currentPoint.localScale.x) <= threshold)
        {
            return true;
        }
        return false;
    }

    // Determines if moving panel's X, Y, and Z position is within range of target position
    protected bool PosInRange()
    {
        if(XposInRange() && YposInRange() && ZposInRange())
        {
            return true;
        }
        return false;
    }
}
