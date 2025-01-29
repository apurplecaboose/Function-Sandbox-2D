using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Doodler : MonoBehaviour
{
    LineRenderer _LineRend;
    List<Vector2> _PointsList;
    EdgeCollider2D _EdgeCollider;
    void Awake()
    {
        _LineRend = this.GetComponent<LineRenderer>();
        _EdgeCollider = this.GetComponent<EdgeCollider2D>();
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

        List<Vector2> edges = new List<Vector2>();
        for( int i  = 0; i < _PointsList.Count; i++)
        {
            edges.Add(_PointsList[i]);
        }
        _EdgeCollider.SetPoints(edges);
    }
    public void SetLineColor(Color targetColor, float targetAlpha)
    {
        _LineRend.colorGradient = SingleColorToGradient(targetColor, targetAlpha);
        if (_EdgeCollider != null)
        {
            Destroy(_EdgeCollider);
            Destroy(this.GetComponent<Rigidbody2D>());
            return;
        }
        else return;
    }
    /// <summary>
    /// Ignore this function its manages sorting layer
    /// </summary>
    public void SetSortingOrder(int orderInLayer)
    {
        _LineRend.sortingOrder = orderInLayer;
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
}
