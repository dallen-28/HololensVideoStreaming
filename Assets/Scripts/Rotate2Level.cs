using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Rotate2Level : Level
{
    public Rotate2Level()
    {
        titleText = "Rotate - Z Axis";
        panelText = "Grab this panel on either side. Now move your left hand up and right hand down " +
            "to rotate this panel about the Z axis. Use this technique to overlay this panel on the one to the left. ";

        startingPoint = GameObject.Find("Rotate2Start").GetComponent<Transform>();
        targetPoint = GameObject.Find("Rotate2Target").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = LevelNumber.Rotate2;
    }

    public override bool CheckForCompletion()
    {
        if (ZRotInRange() && PosInRange())
        {
            return true;
        }
        return false;
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = true;
        //movingPanel.GetComponent<BoundsControl>().enabled = true;

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.TwoHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;

        // Enable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = true;

        // Disable rotation Y and X axis constraint
        movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = AxisFlags.YAxis| AxisFlags.XAxis;

    }
}
