using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : Level
{
    public EndLevel()
    {
        titleText = "Congratulations!";
        panelText = "You have completed the two-handed gesture interaction tutorial. Your total time was: ";

        startingPoint = GameObject.Find("EndStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("FinalTarget").GetComponent<Transform>();

        currentPoint = startingPoint;

        levelNumber = LevelNumber.End;

        //Debug.Log("Total time: ");
    }

    public override bool CheckForCompletion()
    {
        return false;
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = false;
        //movingPanel.GetComponent<BoundsControl>().enabled = false;

    }
    //public override string FormattedText(float time)
    //{
    //    return "";
    //}
}
