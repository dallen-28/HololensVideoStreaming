using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTest : MonoBehaviour
{
    public readonly static Queue<Action> ReceiveVideoQueue = new Queue<Action>();
    
    // Store queue of int variables
    public readonly static Queue<int> a = new Queue<int>();

    // Start is called before the first frame update
    void Start()
    {
        int a = 1;
        ReceiveVideoQueue.Enqueue(() => 
        {
            Debug.Log("Value of a is: " + a.ToString());
        });

        a = 2;

        ReceiveVideoQueue.Dequeue().Invoke();

        a = 3;

        ReceiveVideoQueue.Dequeue().Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
