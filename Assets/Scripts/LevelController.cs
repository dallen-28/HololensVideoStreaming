using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

public class LevelController : MonoBehaviour
{
    public GameObject movingPanel;
    public GameObject targetPanel;
    public GameObject lightBulb;

    Level currentLevel;

    bool pauseTrigger;

    // Start is called before the first frame update
    void Start()
    {
        //targetPanel.SetActive(false);
        //lightBulb.SetActive(false);

        pauseTrigger = false;


        // Start with starting level
        //currentLevel = new StartingLevel();
        currentLevel = new ScaleLevel();
        SetLevelParameters();
    }
    void SetLevelParameters()
    {
        currentLevel.SetManipulationType(movingPanel);

        // Set moving panel transform
        movingPanel.transform.SetPositionAndRotation(currentLevel.startingPoint.position, currentLevel.startingPoint.rotation);
        movingPanel.transform.localScale = currentLevel.startingPoint.localScale;

        // Set target panel transform
        targetPanel.transform.SetPositionAndRotation(currentLevel.targetPoint.position, currentLevel.targetPoint.rotation);
        targetPanel.transform.localScale = currentLevel.targetPoint.localScale;

        // Set panel text
        movingPanel.GetComponentInChildren<TextMeshProUGUI>().SetText(currentLevel.formattedText());

        // Set lightbulb colour to red
        lightBulb.GetComponent<MeshRenderer>().material.color = Color.red;

    }

    // Update is called once per frame
    void Update()
    {
        // Change from complete pause to new message and wait three seconds until next round

        if(pauseTrigger)
        {
            PauseGame(3.0f);
            // To Do: increment level and automatically instantiate class accordingly
            if (currentLevel.levelNumber == 2)
            {
                currentLevel = new ScaleLevel();
                SetLevelParameters();
            }
            else if (currentLevel.levelNumber == 3)
            {
                // To do
            }
        }

        // Synchronize level state with moving panel transform
        currentLevel.currentPoint.SetPositionAndRotation(movingPanel.transform.position, movingPanel.transform.rotation);
        currentLevel.currentPoint.localScale = movingPanel.transform.localScale;

        if (currentLevel.CheckForCompletion())
        {
            // Change lightbuilb colour
            // Wait 3 seconds
            // start next level
            lightBulb.GetComponent<MeshRenderer>().material.color = Color.green;

            // Signal to move to next level on next frame
            pauseTrigger = true;
            
     
        }
    }
    public void OnClick()
    {
        Destroy(currentLevel.nextButton);
        targetPanel.SetActive(true);
        lightBulb.SetActive(true);
        //currentLevel.nextButton.SetActive(false);
        currentLevel = new TranslateLevel();
        SetLevelParameters();
    }
 
    public void PauseGame(float pauseTime)
    {
        Debug.Log("Inside PauseGame()");
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
        }
        Time.timeScale = 1f;
        Debug.Log("Done with my pause");

        pauseTrigger = false;
    }
}
