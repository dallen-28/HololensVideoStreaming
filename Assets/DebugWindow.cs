using System.Collections.Concurrent;
using UnityEngine;
using TMPro;

public class DebugWindow : MonoBehaviour
{
    TextMeshPro textMesh;
    private ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

    public static DebugWindow instance;

    private void Awake()
    {
        instance = this;
    }
    public static void LogThreadSafe(string message)
    {
        if (instance != null)
        {
            instance.logQueue.Enqueue(message);
        }
    }

    // Use this for initialization
    void Start()
    {
        textMesh = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        logQueue.Enqueue(message);
    }
    private void Update()
    {
        while (logQueue.TryDequeue(out var message))
        {
            if (textMesh.text.Length > 600)
            {
                textMesh.text = message + "\n";
            }
            else
            {
                textMesh.text += message + "\n";
            }
        }
    }
}
