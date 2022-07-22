using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine.UI;

public class Rotate1Level : Level
{

    public Rotate1Level()
    {
        titleText = "Rotate - Y Axis";
        panelText = "Grab this panel on either side. Now push and pull with your right and left hands " +
            "to rotate this panel about the y axis. Use this technique to overlay this panel on the one to the left. ";

        startingPoint = GameObject.Find("Rotate1Start").GetComponent<Transform>();
        targetPoint = GameObject.Find("Rotate1Target").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = LevelNumber.Rotate1;
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
        //movingPanel.GetComponent<BoundsControl>().enabled = true;

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.TwoHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;

        // Enable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = true;

        // Disable rotation X and Z axes constraint
        movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;

        GameObject.Find("CoordinateSystem").GetComponent<Image>().enabled = true;
    }
}
