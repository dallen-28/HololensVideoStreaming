using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLevel : Level
{
    public string panelText2;
    public string panelText3;

    public StartingLevel()
    {
        titleText = "Two handed Gesture Interaction Tutorial";
        panelText = "Click on the \"Anchor\" button in the bottom right to anchor the panel in the room. " +
            "Make you sure have an unobstructed view of the panel in front of you and the one to the left. ";

        panelText2 = "This tutorial will show you the basics of two-handed interactions in the Hololens 2. " +
            "Each round will require you to overlay the panel in front of you with the one on the left using a series of two-handed gesture techniques. " +
            "The red light displayed above the target panel will turn green once you have overlayed the panel correctly and can let go to proceed to the next round." +
            "Click next to proceed";

        panelText3 = "You must use two hands to interact with the panel. If your air tap was not registered, the cursor will remain a hollow circle instead of " +
            "turning solid. If this occurs simply repeat the air tap gesture. If you have any questions, ask Daniel or Nadia before starting." +
            "If you are ready, press start to begin.";


        
        startingPoint = GameObject.Find("StartStart").GetComponent<Transform>();
        targetPoint = GameObject.Find("StartTarget").GetComponent<Transform>();

        currentPoint = startingPoint;

        levelNumber = LevelNumber.Start;
        nextButton = GameObject.Find("NextButton");

        // Don't need target point for starting panel
    }

    public override void SetManipulationType(GameObject movingPanel)
    {
        movingPanel.GetComponent<ObjectManipulator>().enabled = false;
        //movingPanel.GetComponent<BoundsControl>().enabled = false;
    }

    public override bool CheckForCompletion()
    {
        // Only triggers completion upon button click
        return false;
    }

    public void SetActivePanels(GameObject targetPanel, GameObject lightbulb)
    {
        targetPanel.SetActive(true);
        lightbulb.SetActive(false);
    }

    public string FormattedText(string pText)
    {
        return "<size=42><b>" + titleText + "</b></size>\n\n" + pText;
    }
}
