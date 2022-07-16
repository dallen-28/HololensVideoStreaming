using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour
{
    public int width;
    public int height;

    // Start is called before the first frame update
    void Start()
    { 
        UpdateAspectRatio(width, height);
    }
    public void UpdateAspectRatio(int width, int height)
    {
        float aspectRatio = (float)width / (float)height;
        this.transform.localScale = new Vector3(aspectRatio / 2, (float)0.5, 1);
    }
}
