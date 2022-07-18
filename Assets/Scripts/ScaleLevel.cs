using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLevel : Level
{
    public ScaleLevel()
    {
        titleText = "Scale";
        panelText = "Using both of your hands, grab this panel by airtapping with both of your hands at any two points. " +
            "Move both hands away from each other until the panel is in the desired position. Release both hands once desired target is reached.";

        startingPoint = GameObject.Find("ScaleStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("ScaleTarget").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = 3;

    }

    public override bool CheckForCompletion()
    {
        if (Mathf.Abs(targetPoint.localScale.x - currentPoint.localScale.x) <= 0.01f)
        {
            return true;
        }
        return false;
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = true;
        movingPanel.GetComponent<BoundsControl>().enabled = true;

        // Enable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = true;

        // Disable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = false;

        //movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.XAxis;
    }
}
