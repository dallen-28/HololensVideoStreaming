using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ScaleLevel : Level
{
    public ScaleLevel()
    {
        titleText = "Scale";
        panelText = "Grab this panel on either side. Move both hands away from each other until " +
            "the panel is the desired size, and then translate it to overlay it with the panel on the left.";

        startingPoint = GameObject.Find("ScaleStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("ScaleTarget").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = LevelNumber.Scale;

    }

    public override bool CheckForCompletion()
    {
        if (PosInRange() && ScaleInRange())
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

        // Disable scale constraint
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = false;

        //movingPanel.GetComponent<RotationAxisConstraint>().ConstraintOnRotation = Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.XAxis;
    }
}
