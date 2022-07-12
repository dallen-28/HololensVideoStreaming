using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTexture : MonoBehaviour
{

    WebCamDevice webCam;
    WebCamTexture webCamTexture;
    Texture2D currentTexture;

    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = new WebCamTexture(1920,1080);
        webCamTexture.Play();
        Debug.Log(webCamTexture.height);
        Debug.Log(webCamTexture.width);
    }

    // Update is called once per frame
    void Update()
    {
        webCamTexture.Play();
        this.GetComponent<MeshRenderer>().material.mainTexture = GetTexture2DFromWebcamTexture(this.webCamTexture);
    }
    public Texture2D GetTexture2DFromWebcamTexture(WebCamTexture webCamTexture)
    {
        // Create new texture2d
        Texture2D tx2d = new Texture2D(webCamTexture.width, webCamTexture.height);
        // Gets all color data from web cam texture and then Sets that color data in texture2d
        tx2d.SetPixels(webCamTexture.GetPixels());
        // Applying new changes to texture2d
        tx2d.Apply();
        return tx2d;
    }
}
