using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class Rotate1Level : Level
{

    public Rotate1Level()
    {
        titleText = "Rotate - Y Axis";
        panelText = "Air tap this panel on either side using both of your hands. Now push and pull with your right and left hands " +
            "to rotate this panel about the y axis. Use this technique to overlay this panel on the one to the left. ";

        startingPoint = GameObject.Find("Rotate1Start").GetComponent<Transform>();
        targetPoint = GameObject.Find("Rotate1Target").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = 3;
    }

    public override bool CheckForCompletion()
    {
        if (YRotInRange() && PosInRange())
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

        // Enable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = true;

        // Disable rotation Y axis constraint
        movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.YAxis;
        //movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.ZAxis;
    }


}
