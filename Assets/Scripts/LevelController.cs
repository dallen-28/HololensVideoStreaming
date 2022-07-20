using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;

public class LevelController : MonoBehaviour
{
    public GameObject movingPanel;
    public GameObject targetPanel;
    public GameObject lightBulb;
    public GameObject mainCamera;

    Level currentLevel;
    Level startLevel;
    Level translateLevel;
    Level scaleLevel;
    Level rotate1Level;
    Level rotate2Level;
    Level rotate3Level;
    Level finalLevel;
    Level endLevel;
    Queue<Level> levelQueue;

    Matrix4x4 anchorPos;

    bool pauseTrigger;
    bool manipulationActive;

    float startTime;
    float endTime; 

    // Start is called before the first frame update
    void Start()
    {

        levelQueue = new Queue<Level>();

        startLevel = new StartingLevel();
       
        //startLevel.SetActivePanels(targetPanel, lightBulb);

        translateLevel = new TranslateLevel();
        scaleLevel = new ScaleLevel();
        rotate1Level = new Rotate1Level();
        rotate2Level = new Rotate2Level();
        rotate3Level = new Rotate3Level();
        finalLevel = new FinalLevel();
        endLevel = new EndLevel();

        levelQueue.Enqueue(startLevel);
        //levelQueue.Enqueue(translateLevel);
        levelQueue.Enqueue(scaleLevel);
        levelQueue.Enqueue(rotate1Level);
        levelQueue.Enqueue(rotate2Level);
        levelQueue.Enqueue(rotate3Level);
        levelQueue.Enqueue(finalLevel);
        levelQueue.Enqueue(endLevel);

        //levelQueue.Enqueue(finalLevel);
        //levelQueue.Enqueue(endLevel);

        pauseTrigger = false;
        manipulationActive = false;


        // Start with starting level
        //currentLevel = new StartingLevel();
        //currentLevel = new ScaleLevel();


        targetPanel.SetActive(false);
        lightBulb.SetActive(false);

        currentLevel = levelQueue.Dequeue();
        //currentLevel = scaleLevel;
        //currentLevel = finalLevel;
            

        SetLevelParameters();

    }
    void SetLevelParameters()
    {
        if(currentLevel.levelNumber == 7)
        {
            endTime = Time.time - startTime;
            movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(currentLevel.FormattedText() + GetFormattedTime(endTime));
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

        // If starting level look at 
        if(currentLevel.levelNumber == 1)
        {
            //Debug.Log(movingPanel.transform.position.ToString());
            Vector3 panelOffset = new Vector3(0, 0, 0.7f);
            Vector3 buttonOffset = new Vector3(0.26f, -0.17f, 0.7f);
            Vector3 newPanelPos = mainCamera.transform.localToWorldMatrix.MultiplyPoint(panelOffset);
            Vector3 newButtonPos = mainCamera.transform.localToWorldMatrix.MultiplyPoint(buttonOffset);
            Quaternion newRot = mainCamera.transform.rotation;

            movingPanel.transform.SetPositionAndRotation(newPanelPos, newRot);
            currentLevel.nextButton.transform.SetPositionAndRotation(newButtonPos, newRot);
            //movingPanel.transform.LookAt(new Vector3(0,0,0));
        }


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
        anchorPos = mainCamera.transform.localToWorldMatrix;

        GameObject.Find("AnchorOffset").transform.position = mainCamera.transform.position;
        GameObject.Find("AnchorOffset").transform.rotation = mainCamera.transform.rotation;

        Destroy(currentLevel.nextButton);
        targetPanel.SetActive(true);
        lightBulb.SetActive(true);
        //currentLevel.nextButton.SetActive(false);
        currentLevel = levelQueue.Dequeue();
        SetLevelParameters();

        startTime = Time.time;
    }
}
