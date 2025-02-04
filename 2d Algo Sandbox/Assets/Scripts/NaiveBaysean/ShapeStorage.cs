using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CookedTrainingData", menuName = "ScriptableObjects/Cooked", order = 1)]
public class ShapeStorage : ScriptableObject
{
    public enum ShapeToMatch
    {
        NULL,
        Circle,
        Square,
        Triange
    }
    //identifiers
    [Header("INPUTS HERE")]
    public ShapeToMatch CurrentShape;

    //raw data
    public List<RawTrainingData> RawData;
    [Header("For DEBUG Only")]
    public bool do_nothingbool_LOL;
    public List<float> raw_sig_top, raw_sig_bottom, raw_sig_left, raw_sig_right, raw_middleweight;
    [Header ("ProcessedData DEBUG")]
    public bool do_nothingbool_LOL2;
    //processed data
    public float mean_top, mean_bottom, mean_left, mean_right, mean_middle;
    public float std_top, std_bottom, std_left, std_right, std_middle;
}
