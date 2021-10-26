using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class UpdateCTSlice : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private ArrayList imageBuffer;

    public string view = "Axial";
    public int numSlices = 30;

    void Start()
    {
        // Set Scale
        this.transform.localScale = new Vector3((float)0.260148, (float)0.1907752, (float)0.01118677);
        this.transform.localRotation = Quaternion.Euler(0,0,180);

        this.imageBuffer = new ArrayList();
        this.meshRenderer = this.GetComponent<MeshRenderer>();

        string folderPath = string.Concat("Textures/", view, "CTSlices");

        Object[] textures = Resources.LoadAll(folderPath, typeof(Texture2D));


        for (int i = 0; i < textures.Length; i++)
        {
            this.imageBuffer.Add(textures[i]);
        }

        // Display first texture on Axial display
        var tex = new Texture2D(500, 374, TextureFormat.RGB24, false);
        tex = (Texture2D)this.imageBuffer[0];
        //tex.Apply(updateMipmaps: false);
        meshRenderer.material.mainTexture = tex;

    }
    public void OnSliderChange(SliderEventData eventData)
    {
        int value = (int)Mathf.Round(eventData.NewValue * numSlices);
        var tex = new Texture2D(500, 374, TextureFormat.RGB24, false);
        tex = (Texture2D)this.imageBuffer[value];
        //tex.Apply(updateMipmaps: false);
        meshRenderer.material.mainTexture = tex;
    }
}
