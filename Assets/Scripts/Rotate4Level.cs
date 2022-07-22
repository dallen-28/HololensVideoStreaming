using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Rotate4Level : Level
{
    public Rotate4Level()
    {
        titleText = "Rotate - All axes";
        //panelText = "This last type of rotation will be easiest to utilize the one-handed technique" +
        //    "you learned in the \"Tips\" Tutorial. With one hand, air-tap to grab the ball at the top middle of the panel, and push or " +
        //    "pull back to rotate the panel about the X - axis. Use this technique to overlay the panel to the one to your left.";

        panelText = "Grab the panel in the middle with either hand" +
            " and rotate your wrist to rotate the panel about all 3 axes. Use this technique " +
            "to overlay the panel to the one on your left.";

        startingPoint = GameObject.Find("Rotate4Start").GetComponent<Transform>();
        targetPoint = GameObject.Find("Rotate4Target").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = LevelNumber.Rotate4;
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
        //movingPanel.GetComponent<BoundsControl>().enabled = true;

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.OneHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;

        // Enable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = true;

        // Disable rotation Y and Z axis constraint
        movingPanel.GetComponent<RotationAxisConstraint>().enabled = false;
    }
}
