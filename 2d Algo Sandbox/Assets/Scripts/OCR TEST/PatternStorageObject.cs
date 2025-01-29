using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReferenceShape", menuName = "ScriptableObjects/New Reference Shape", order = 1)]
public class PatternStorageObject :  ScriptableObject
{
    public bool IsOpenShape;
    public string Pattern_Name;
    public List<Vector2> ReferenceShapeData;
}
