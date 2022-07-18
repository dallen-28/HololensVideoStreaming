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
        panelText = "Congratulations! You have completed the two-handed gesture interaction tutorial. Your total time was: ";

        Debug.Log("Total time: ");
    }

    public override bool CheckForCompletion()
    {
        throw new System.NotImplementedException();
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        throw new System.NotImplementedException();
    }
}
