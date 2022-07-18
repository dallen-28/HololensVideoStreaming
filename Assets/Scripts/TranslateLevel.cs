using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateLevel : Level
{
    public TranslateLevel()
    {
        titleText = "Translate";
        panelText = "Using both of your hands, grab this panel by airtapping with both of your hands at any two points. " +
            "Move both hands together until the panel is in the desired position. Release both hands once desired target is reached.";
        
        startingPoint = GameObject.Find("TranslateStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("TranslateTarget").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = 2;
  
    }

    public override bool CheckForCompletion()
    {
        if(XposInRange() && YposInRange() && ZposInRange())
        {
            return true;
        }
        return false;
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = true;
        movingPanel.GetComponent<BoundsControl>().enabled = true;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;
    }

    // Determines if moving panel's X position is within range of the target position
    public bool XposInRange()
    {
        if (Mathf.Abs(targetPoint.position.x - currentPoint.position.x) <= 0.01f)
        {
            return true;
        }
        return false;
    }

    // Determines if moving panel's Y position is within range of target position
    public bool YposInRange()
    {
        if (Mathf.Abs(targetPoint.position.y - currentPoint.position.y) <= 0.01f)
        {
            return true;
        }
        return false;
    }

    // Determine's if moving panel's Z position is within range of target position
    public bool ZposInRange()
    {
        if (Mathf.Abs(targetPoint.position.z - currentPoint.position.z) <= 0.01f)
        {
            return true;
        }
        return false;
    }
}
