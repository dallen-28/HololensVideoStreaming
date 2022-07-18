using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLevel : Level
{
    public StartingLevel()
    {
        titleText = "Two handed Gesture Interaction Tutorial";
        panelText = "You have learned the basics of hand gestures in the Hololens 2. This tutorial will now show you the basics of two handed-interactions. " +
            "Each round will require you to overlay the panel on the right with the one on the left using a series of two-handed gesture techniques. " +
            "Click the start button in the bottom right corner to begin";
        
        startingPoint = GameObject.Find("StartStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("StartStart").GetComponent<Transform>();

        currentPoint = startingPoint;

        levelNumber = 1;
       
        // Set next level button visible
        nextButton = GameObject.Find("NextButton");
        nextButton.SetActive(true);

        // Don't need target point for starting panel
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = false;
        movingPanel.GetComponent<BoundsControl>().enabled = false;
    }

    public override bool CheckForCompletion()
    {
        // Only triggers completion upon button click
        return false;
    }

    public void SetActivePanels(GameObject targetPanel, GameObject lightbulb)
    {
        targetPanel.SetActive(false);
        lightbulb.SetActive(false);
    }

}
