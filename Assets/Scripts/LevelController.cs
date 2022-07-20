using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;

public class LevelController : MonoBehaviour
{

    // Public GameObjects
    public GameObject movingPanel;
    public GameObject targetPanel;
    public GameObject lightBulb;

    // Starting level
    StartingLevel startLevel;

    // Remaining Levels
    Level currentLevel;
    Level translateLevel;
    Level scaleLevel;
    Level rotate1Level;
    Level rotate2Level;
    Level rotate3Level;
    Level finalLevel;
    Level endLevel;
    Queue<Level> levelQueue;

    // Anchor position
    Matrix4x4 anchorPos;

    // Flags
    bool manipulationActive;
    bool anchorSet;

    // Button Click Counter
    int buttonClickCounter;

    // Timer variables
    float startTime;
    float endTime;

    // Panel  and buton fffset for starting position
    Vector3 panelOffset;
    Vector3 buttonOffset;
    Vector3 targetPanelOffset;

    // Start is called before the first frame update
    void Start()
    {

        //panelOffset = new Vector3(0, 0, 1.5f);
        //buttonOffset = new Vector3(0.26f, -0.17f, 1.5f);
        //targetPanelOffset = new Vector3(-1.3f, 0, 1.5f);

        // Create level queue
        levelQueue = new Queue<Level>();

        // Create our levels desired levels
        translateLevel = new TranslateLevel();
        scaleLevel = new ScaleLevel();
        rotate1Level = new Rotate1Level();
        rotate2Level = new Rotate2Level();
        rotate3Level = new Rotate3Level();
        finalLevel = new FinalLevel();
        endLevel = new EndLevel();

        // Enqueue our levels in the desired order
        levelQueue.Enqueue(translateLevel);
        levelQueue.Enqueue(scaleLevel);
        levelQueue.Enqueue(rotate1Level);
        levelQueue.Enqueue(rotate2Level);
        levelQueue.Enqueue(rotate3Level);
        levelQueue.Enqueue(finalLevel);
        levelQueue.Enqueue(endLevel);
        
        // Set manipulation active flag to false
        manipulationActive = false;
        anchorSet = false;

        // Set button click counter to 0
        buttonClickCounter = 0;

        // Start with starting level
        startLevel = new StartingLevel();
        startLevel.SetActivePanels(targetPanel, lightBulb);
        startLevel.nextButton.SetActive(true);

        movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(startLevel.panelText);

        currentLevel = startLevel;

        //targetPanel.SetActive(false);
        //lightBulb.SetActive(false);

        //currentLevel = levelQueue.Dequeue();
        //currentLevel = scaleLevel;
        //currentLevel = finalLevel;
            

        //SetLevelParameters();

    }
    void SetLevelParameters()
    {
        if(currentLevel.levelNumber == Level.LevelNumber.End)
        {
            endTime = Time.time - startTime;
            movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(currentLevel.FormattedText() + GetFormattedTime(endTime));
            targetPanel.SetActive(false);
        }
        else
        {
            // Set panel text
            movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(currentLevel.FormattedText());
        }

        currentLevel.SetManipulationType(movingPanel);

    

        Matrix4x4 localMovingPanelPos = anchorPos * currentLevel.startingPoint.localToWorldMatrix;
        Matrix4x4 localTargetPanelPos = anchorPos * currentLevel.targetPoint.localToWorldMatrix;

        // Set moving panel transform
        movingPanel.transform.SetPositionAndRotation((Vector3)localMovingPanelPos.GetColumn(3), localMovingPanelPos.rotation);
        movingPanel.transform.localScale = currentLevel.startingPoint.localScale;

        // Set target panel transform
        targetPanel.transform.SetPositionAndRotation((Vector3)localTargetPanelPos.GetColumn(3), localTargetPanelPos.rotation);
        targetPanel.transform.localScale = currentLevel.targetPoint.localScale;

        currentLevel.UpdateTargetTransform((Vector3)localTargetPanelPos.GetColumn(3), localTargetPanelPos.rotation);

        // Set lightbulb colour to red
        lightBulb.GetComponent<MeshRenderer>().material.color = Color.red;

    }
    string GetFormattedTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        //here backslash is must to tell that colon is
        //not the part of format, it just a character that we want in output
        return time.ToString(@"mm\:ss");
    }

    // Update is called once per frame
    void Update()
    {       
        // Synchronize level state with moving panel transform
        currentLevel.currentPoint.SetPositionAndRotation(movingPanel.transform.position, movingPanel.transform.rotation);
        currentLevel.currentPoint.localScale = movingPanel.transform.localScale;


        // If panel overlayed set lightbulb colour to green
        if (currentLevel.CheckForCompletion())
        {

            lightBulb.GetComponent<MeshRenderer>().material.color = Color.green;

            // If panel overlayed and manipulation ended move to next level
            if (!manipulationActive)
            {
                currentLevel = levelQueue.Dequeue();
                SetLevelParameters();
            }

        }
        else
        {
            lightBulb.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        //Debug.Log(manipulationActive);
    }

    public void OnManipulationStarted()
    {
        manipulationActive = true;
    }
    public void OnManipulationEnded()
    {
        manipulationActive = false;
    }

    public void OnClick()
    {
        //To Do: Set up panel text Queue in starting level to avoid hardcoded number of button clicks

        if(buttonClickCounter == 2)
        {
            Destroy(currentLevel.nextButton);
            targetPanel.SetActive(true);
            lightBulb.SetActive(true);
            //currentLevel.nextButton.SetActive(false);
            currentLevel = levelQueue.Dequeue();
            SetLevelParameters();

            startTime = Time.time;
        }
        
        // Update panel and button text
        else if (buttonClickCounter == 1)
        {
            movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(startLevel.FormattedText(startLevel.panelText3));

            currentLevel.nextButton.GetComponentInChildren<TextMeshPro>().SetText("Start");
            buttonClickCounter++;
        }

        // Set anchor position, update panel text, update button text
        else
        {
            anchorPos = Camera.main.transform.localToWorldMatrix;
            movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(startLevel.FormattedText(startLevel.panelText2));

            currentLevel.nextButton.GetComponentInChildren<TextMeshPro>().SetText("Next");
            buttonClickCounter++;
            anchorSet = true;
        }
    }
}
