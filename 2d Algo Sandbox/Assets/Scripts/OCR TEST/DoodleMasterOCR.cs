using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class DoodleMasterOCR : MonoBehaviour
{
    public float MaxLineLength = 1500;
    public DoodleOCR DoodlerPrefab;
    DoodleOCR _currentDoodler;
    int _DoodleSortOrder = 50;

    public List<Vector3> DrawingOutput; // x,y,doodlenumber
    public PatternStorageObject PatternScriptableObject;

    [SerializeField] List<DoodleOCR> _DoodlesList;
#if UNITY_EDITOR
    // Saves data written to scriptable objects through code.
    void LetsGetDirty(UnityEngine.Object scriptableObject)
    {
        EditorUtility.SetDirty(scriptableObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //print("finished getting dirty");
    }
#endif
    void TEMPFUNCTIONS()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ClearDoodles();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // USED for transfering pattern to a list scriptable Object
            List<Vector2> tempoutputlist = new List<Vector2>();
            foreach (Vector3 datapoint in DrawingOutput)
            {
                if (datapoint.z == 0)
                {
                    tempoutputlist.Add(new Vector2(datapoint.x, datapoint.y));
                }
            }
            PatternScriptableObject.ReferenceShapeData = tempoutputlist;
#if UNITY_EDITOR
            LetsGetDirty(PatternScriptableObject);
#endif
        }
    }
    void Update()
    {
        TEMPFUNCTIONS();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _currentDoodler = Instantiate(DoodlerPrefab, this.transform); // instantiate as a child of the doodle master.
            _DoodlesList.Add(_currentDoodler); // adds all doodles to a list for easy clearing
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_currentDoodler == null) return;
            EndDoodlin();
        }
        if (_currentDoodler != null)
        {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currentDoodler.UpdateLine(mousepos);
            if (_currentDoodler.IsLineTooLong(MaxLineLength)) EndDoodlin();
        }
    }
    void EndDoodlin()
    {
        DrawingOutput.AddRange(_currentDoodler.PrintPointOutput(0));
        _currentDoodler.SetLineColor(Color.cyan); //optional color change
        _currentDoodler = null;
    }
    public void SetFinalColor(Color targetColor, float targetAlpha)
    {
        // SUBSIGIL DRAWING DONE
        foreach (DoodleOCR d in _DoodlesList)
        {
            d.SetLineColor(targetColor);
        }
    }
    public void ClearDoodles()
    {
        print("CLEARING DOODLES!!!");
        foreach (DoodleOCR d in _DoodlesList)
        {
            Destroy(d.gameObject);
        }
        _DoodlesList.Clear();
        DrawingOutput.Clear();
    }
}
