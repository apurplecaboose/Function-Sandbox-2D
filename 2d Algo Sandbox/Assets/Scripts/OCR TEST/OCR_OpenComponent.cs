using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OCR_OpenComponent : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    public List<Vector2> _DoodlePointOutputVector2;
    public bool donefilling;

    void DoodleNumberSorting()
    {
        int doodNum = 0;
        foreach (Vector3 datapoint in DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == doodNum)
            {
                _DoodlePointOutputVector2.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
        float thresholdvalue = 0.2f;
        if (Vector2.Distance(_DoodlePointOutputVector2[0], _DoodlePointOutputVector2[_DoodlePointOutputVector2.Count - 1]) < thresholdvalue)
        {
            print("CLOSED!!!!!!!!!");
        }
        else
        {
            print("OPENNNNNN");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
    }
}
