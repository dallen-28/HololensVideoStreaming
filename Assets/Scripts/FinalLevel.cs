using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;

public class FinalLevel : Level
{
    public FinalLevel()
    {
        titleText = "Final Level";
        panelText = "You have made it to the final level! Use all of the techniques you have learned in this " +
            "tutorial to overlay the panel on the one to the left";

        startingPoint = GameObject.Find("FinalStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("FinalTarget").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = 6;
    }

    public override bool CheckForCompletion()
    {    
        if (XRotInRange() && YRotInRange() && PosInRange() && ScaleInRange())
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
        movingPanel.GetComponent<MinMaxScaleConstraint>().enabled = false;

        // Disable Z axis rotation constraint
        movingPanel.GetComponent<RotationAxisConstraint>().enabled = false;
    }
}
