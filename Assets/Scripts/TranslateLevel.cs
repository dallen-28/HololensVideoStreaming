using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class TranslateLevel : Level
{
    public TranslateLevel()
    {
        titleText = "Translate";
        panelText = "Using both of your hands, grab this panel by airtapping with both of your hands on either side. " +
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

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.TwoHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;
    }
}
