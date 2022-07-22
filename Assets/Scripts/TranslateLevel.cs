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
        panelText = "Grab this panel by airtapping on either side. " +
            "Move both hands together until the panel is overlayed on the target panel. Release both hands once the light turns green.";
        
        startingPoint = GameObject.Find("TranslateStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("TranslateTarget").GetComponent<Transform>();

        currentPoint = startingPoint;
        levelNumber = LevelNumber.Translate;
  
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
        //movingPanel.GetComponent<BoundsControl>().enabled = true;

        movingPanel.GetComponent<ObjectManipulator>().ManipulationType = ManipulationHandFlags.TwoHanded;

        // Disable move constraint
        movingPanel.GetComponent<MoveAxisConstraint>().enabled = false;
    }
}
