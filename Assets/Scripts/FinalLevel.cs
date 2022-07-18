using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        throw new System.NotImplementedException();
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        throw new System.NotImplementedException();
    }
}
