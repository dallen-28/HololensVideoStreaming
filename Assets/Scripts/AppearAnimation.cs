using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAnimation : MonoBehaviour
{
    public int stepSize = 5;
    public int growSize = 1;

    bool active = false;
    bool grow = false;

    // Start is called before the first frame update
    void Start()
    {
        // Start off as shrunken
        this.transform.localScale = new Vector3((float)0.00001, (float)0.00001, (float)0.00001);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.localPosition;
        Vector3 scale = this.transform.localScale;

        if(this.active == true)
        {
            if (this.grow == true)
            {
                if (scale.x >= 0.001)
                {
                    this.active = false;
                }
                else
                {
                    this.transform.localScale = new Vector3((float)(scale.x + growSize * 0.00001), (float)(scale.y + growSize * 0.00001), (float)(scale.z + growSize * 0.00001));
                }
            }
            else
            {
                if (scale.x <= 0.00001)
                {
                    this.active = false;
                }
                else
                {
                    this.transform.localScale = new Vector3((float)(scale.x - growSize * 0.00001), (float)(scale.y - growSize * 0.00001), (float)(scale.z - growSize * 0.00001));
                }
            }
        }

        //this.transform.position = new Vector3(pos.x, pos.y, (float)(pos.z - stepSize*0.001));
            
        //this.transform.localScale *= (float)(growSize*0.001);// = scale*3;
        //this.transform.localScale = (float)(scale*0.003);//(float)(growSize * 0.001);// = scale*3;


    }
    public void OnButtonPressed()
    {
        Debug.Log("Pressed");
        this.active = true;
        if(this.grow)
        {
            this.grow = false;
        }
        else
        {
            this.grow = true;
        }
    }
}
