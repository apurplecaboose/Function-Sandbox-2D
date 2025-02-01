using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DebugREE : MonoBehaviour
{
    public bool onswitch;
    public PatternStorageObject data;
    void OnDrawGizmos()
    {
        if (onswitch)
        {
            foreach (Vector2 vec2point in data.ReferenceShapeData)
            {
                Gizmos.DrawSphere(new Vector3(vec2point.x, vec2point.y, 0), 200f);
            }
        }       
    }
}
