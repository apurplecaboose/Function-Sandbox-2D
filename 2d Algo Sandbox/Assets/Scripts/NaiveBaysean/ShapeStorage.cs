using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CookedTrainingData", menuName = "ScriptableObjects/Cooked", order = 1)]
public class ShapeStorage : ScriptableObject
{
    //identifiers
    public string shapeName; // label

    //raw data
    public List<RawTrainingData> RawData;
    public List<float> raw_sig_top, raw_sig_bottom, raw_sig_left, raw_sig_right, raw_middleweight;

    //processed data
    public float mean_top, mean_bottom, mean_left, mean_right, mean_middle;
    public float std_top, std_bottom, std_left, std_right, std_middle;
}
