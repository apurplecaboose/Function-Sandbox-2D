using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OCR_OpenComponent : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    public List<Vector2> _DoodlePointOutputVector2;
    public int SubDoodleNumber = 0;
    void DoodleNumberSorting()
    {
        foreach (Vector3 datapoint in DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == SubDoodleNumber)
            {
                _DoodlePointOutputVector2.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
        float closedthresholdvalue = 0.2f;
        if (Vector2.Distance(_DoodlePointOutputVector2[0], _DoodlePointOutputVector2[_DoodlePointOutputVector2.Count - 1]) < closedthresholdvalue)
        {
            print("CLOSED!!!!!!!!!");
            //then run next check
        }
        else
        {
            print("OPENNNNNN");
            //then run next check
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
