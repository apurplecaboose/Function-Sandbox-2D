using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class DoodleOCR : MonoBehaviour
{
    LineRenderer _LineRend;
    List<Vector2> _PointsList;
    void Awake()
    {
        _LineRend = this.GetComponent<LineRenderer>();
    }
    public void UpdateLine(Vector2 position)
    {
        if (_PointsList == null)
        {
            _PointsList = new List<Vector2>(); //initialize list
            SetPoint(position);
            return;
        }
        int lastindex = _PointsList.Count - 1;
        float minThresholdPointDist = 0.1f;
        if (Vector2.Distance(_PointsList[lastindex], position) > minThresholdPointDist)
        {
            SetPoint(position);
            return;
        }
        else return;
    }
    void SetPoint(Vector2 point)
    {
        _PointsList.Add(point);
        _LineRend.positionCount = _PointsList.Count;
        _LineRend.SetPosition(_PointsList.Count - 1, point);
    }
    public void SetLineColor(Color targetColor)
    {
        _LineRend.colorGradient = SingleColorToGradient(targetColor);
    }
    Gradient SingleColorToGradient(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color, 0.0f);
        colorKeys[1] = new GradientColorKey(color, 1.0f);
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(color.a, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(color.a, 1.0f);
        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }

    public List<Vector3> PrintPointOutput(int doodleNumber)
    {
        List<Vector3> returnList = new List<Vector3>();
        foreach (Vector2 drawpoint in  _PointsList)
        {
            returnList.Add(new Vector3(drawpoint.x, drawpoint.y, doodleNumber));
        }
        return returnList;  
    }
}
