using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class UpdateCTSlice : MonoBehaviour
{
    [Header("Data Loading")]
    [Tooltip("When true, always load from persistentDataPath (even in Editor)")]
    public bool forceFileSystemLoading = false;

    [Header("View Settings")]
    public string view = "Axial";                    // Axial, Coronal, Sagittal
    public string resourcesFolder = "Textures";      // Used in Editor
    public string deviceDataFolder = "CTData";       // Used on HoloLens 2

    private MeshRenderer meshRenderer;
    private List<Texture2D> imageBuffer = new List<Texture2D>();

    public int CurrentSliceIndex { get; private set; } = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Keep your original transform setup
        transform.localScale = new Vector3(0.260148f, 0.1907752f, 0.01118677f);
        transform.localRotation = Quaternion.Euler(0, 0, 180);

        LoadImages();

        if (imageBuffer.Count > 0)
        {
            SetSliceIndex(0);
        }
        else
        {
            Debug.LogError($"[UpdateCTSlice] No images loaded for view '{view}'");
        }
    }

    private void LoadImages()
    {
#if UNITY_EDITOR
        if (!forceFileSystemLoading)
        {
            LoadFromResources();
            return;
        }
#endif
        LoadFromPersistentDataPath();
    }

    // ==================== EDITOR (Resources) ====================
    private void LoadFromResources()
    {
        imageBuffer.Clear();

        string folderPath = $"{resourcesFolder}/{view}CTSlices";
        Object[] textures = Resources.LoadAll(folderPath, typeof(Texture2D));

        foreach (var obj in textures)
        {
            if (obj is Texture2D tex)
                imageBuffer.Add(tex);
        }

        // Sort by name (important for correct slice order)
        imageBuffer.Sort((a, b) => a.name.CompareTo(b.name));

        Debug.Log($"[UpdateCTSlice] Loaded {imageBuffer.Count} slices from Resources for '{view}'");
    }

    // ==================== HOLOLENS 2 (File System) ====================
    private void LoadFromPersistentDataPath()
    {
        imageBuffer.Clear();

        string folderPath = Path.Combine(Application.persistentDataPath, deviceDataFolder, view);

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"[UpdateCTSlice] Folder not found on device: {folderPath}");
            return;
        }

        // Support both PNG and JPG
        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(folderPath, "*.png"));
        files.AddRange(Directory.GetFiles(folderPath, "*.jpg"));
        files.AddRange(Directory.GetFiles(folderPath, "*.jpeg"));

        files.Sort(); // Alphabetical/numerical sort for correct slice order

        foreach (string file in files)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(file);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(fileData))
                {
                    // Set a clean name for debugging
                    tex.name = Path.GetFileNameWithoutExtension(file);
                    imageBuffer.Add(tex);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UpdateCTSlice] Failed to load {file}: {e.Message}");
            }
        }

        Debug.Log($"[UpdateCTSlice] Loaded {imageBuffer.Count} slices from device storage for '{view}'");
    }

    public void SetSliceIndex(int index)
    {
        if (imageBuffer.Count == 0) return;

        CurrentSliceIndex = Mathf.Clamp(index, 0, imageBuffer.Count - 1);
        meshRenderer.material.mainTexture = imageBuffer[CurrentSliceIndex];
    }

    // Called by MRTK Slider
    public void OnSliderChange(SliderEventData eventData)
    {
        if (imageBuffer.Count == 0) return;

        int targetIndex = Mathf.RoundToInt(eventData.NewValue * (imageBuffer.Count - 1));
        SetSliceIndex(targetIndex);
    }
}