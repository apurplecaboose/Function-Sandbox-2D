
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ShapeGroup", menuName = "ScriptableObjects/ShapeGroup", order = 1)]
public class ShapeGroup : ScriptableObject
{
    public enum SubShape
    {
        NULL,
        Circle,
        Square,
        Triangle,
        FishAlpha,
        LetterN,
        LetterV,
        LetterW,
        LetterY,
        LetterZ,
        Star,
        Sigma,
        Swirl,
        Sine,
        Heart,
    }
    public SubShape CurrentShape;
    public List<RawShapes> RawData;

    public float CurrentAlignCost;
}
