using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Rotate3Level : Level
{
    public Rotate3Level()
    {
        titleText = "Rotate - X Axis";
        panelText = "This last type of rotation will be easiest to utilize the one-handed technique" +
            "you learned in the \"Tips\" Tutorial. With one hand, air-tap to grab the ball at the top middle of the panel, and push or " +
            "pull back to rotate the panel about the X - axis. Use this technique to overlay the panel to the one to your left.";

        startingPoint = GameObject.Find("Rotate3Start").GetComponent<Transform>();
        targetPoint = GameObject.Find("Rotate3Target").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = 5;
    }

    public override bool CheckForCompletion()
    {
        if (XRotInRange() && PosInRange())
        {
            return true;
        }
        return false;
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = true;
        movingPanel.GetComponent<BoundsControl>().enabled = true;

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.TwoHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;

        // Enable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = true;

        // Disable rotation Y and Z axis constraint
        movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = AxisFlags.YAxis | AxisFlags.ZAxis;
    }
}
