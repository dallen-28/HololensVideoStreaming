using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public abstract class Level
{
    public GameObject nextButton;

    public string titleText;
    public string panelText;

    public Transform startingPoint;
    public Transform currentPoint;
    public Transform targetPoint;
    public int levelNumber;

    public abstract bool CheckForCompletion();
    public abstract void SetManipulationType(GameObject movingPanel);

    public string formattedText()
    {
        return "<size=42><b>" + titleText + "</b></size>\n\n" + panelText;
    }
}
