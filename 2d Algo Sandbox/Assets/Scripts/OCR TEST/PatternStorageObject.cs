using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReferenceShape", menuName = "ScriptableObjects/New Reference Shape", order = 1)]
public class PatternStorageObject :  ScriptableObject
{
    public List<Vector2> ReferenceShapeData;
}
