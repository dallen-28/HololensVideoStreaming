using System;
using UnityEngine;

public class UpdateTexture : MonoBehaviour
{

    WebCamDevice webCam;
    WebCamTexture webCamTexture;
    Texture2D currentTexture;
    MeshRenderer meshRenderer;
    float pushButtonOffset = 0.2f;

    public int width = 1920;
    public int height = 1080;

    public bool updateTextureFromWebcam = true;

    

    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = new WebCamTexture(width, height);
        meshRenderer = this.GetComponent<MeshRenderer>();

        UpdateAspectRatio(width, height);
    }
    public void UpdateAspectRatio(int width, int height)
    {
        float aspectRatio = (float)height / (float)width;
        this.transform.localScale = new Vector3(1, aspectRatio, 1);

        //OnResize();

    }
    public void OnResize()
    {
        // Update PushButton Position
        float x = this.transform.localPosition.x + this.transform.localScale.x / 2.0f + pushButtonOffset;
        float y = this.transform.localPosition.y;
        float z = this.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        //webCamTexture.Play();
        //meshRenderer.material.mainTexture = GetTexture2DFromWebcamTexture(this.webCamTexture);
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
