using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevDoodler : MonoBehaviour
{
    LineRenderer _LineRend;
    List<Vector2> _PointsList;

    void Awake()
    {
        _LineRend = this.GetComponent<LineRenderer>();
        SetLineColor(Color.magenta, 1);
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
    public void SetLineColor(Color targetColor, float targetAlpha)
    {
        _LineRend.colorGradient = SingleColorToGradient(targetColor, targetAlpha);
    }

    /// <summary>
    /// Ignore this function its sorta useless it just converts a color to a gradient type
    /// </summary>
    Gradient SingleColorToGradient(Color color, float alpha)
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color, 0.0f);
        colorKeys[1] = new GradientColorKey(color, 1.0f);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(alpha, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(alpha, 1.0f);

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }

    public List<Vector3> GenerateAccuracyPoints()
    {
        List<Vector3> outputList = new List<Vector3>();
        for (int i = 0; i < _PointsList.Count; i += 3)
        {
            if (_PointsList.Count % 3 != 0)
            {
                if( i >= _PointsList.Count - 5)
                {
                    break;
                }
            }

            outputList.Add(new Vector3(_PointsList[i].x, _PointsList[i].y, 0));
        }
        return outputList;  
    }
}
