using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OCR_AABB : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    public List<Vector2> _DoodlePointOutputVector2;
    Vector4 AABB_Bounds; 

    void DrawDebugBox(float xMin, float xMax, float yMin, float yMax, Color linecolor, float LineDuration)
    {
        float zvalue = -10;
        Vector3 bottomLeft = new Vector3(xMin, yMin, zvalue);
        Vector3 bottomRight = new Vector3(xMax, yMin, zvalue);
        Vector3 topLeft = new Vector3(xMin, yMax, zvalue);
        Vector3 topRight = new Vector3(xMax, yMax, zvalue);

        Debug.DrawLine(bottomLeft, bottomRight, linecolor, LineDuration,false);
        Debug.DrawLine(bottomRight, topRight, linecolor, LineDuration, false);
        Debug.DrawLine(topRight, topLeft, linecolor, LineDuration, false);
        Debug.DrawLine(topLeft, bottomLeft, linecolor, LineDuration, false);
    }
    void DoodleNumberSorting()
    {
        foreach (Vector3 datapoint in DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == 0)
            {
                _DoodlePointOutputVector2.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }

        AABB_Bounds.w = _DoodlePointOutputVector2.Min((v => v.x));
        AABB_Bounds.x = _DoodlePointOutputVector2.Max((v => v.x));
        AABB_Bounds.y = _DoodlePointOutputVector2.Min((v => v.y));
        AABB_Bounds.z = _DoodlePointOutputVector2.Max((v => v.y));


        DrawDebugBox(AABB_Bounds.w, AABB_Bounds.x, AABB_Bounds.y, AABB_Bounds.z, Color.white, 250f);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
            
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            DrawDebugBox(AABB_Bounds.w, AABB_Bounds.x, AABB_Bounds.y, AABB_Bounds.z, Color.white, 10f);
        }
    }
}
