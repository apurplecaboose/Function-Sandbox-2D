using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DoodleMasterOCR : MonoBehaviour
{
    public DoodleOCR DoodlerPrefab;
    DoodleOCR _currentDoodler;
    [SerializeField] List<DoodleOCR> _DoodlesList; // currently serialized for visibility

    public bool DEV_PATTERN_CREATION_MODE;
    public PatternStorageObject DEV_PatternScriptableObject;

    [HideInInspector]public float CurrentDoodleOpenDistance; //currently being called by OCR K_nearestneighbor could be made private if to combine the two scripts

    public List<Vector3> DrawingOutput; // x,y,doodlenumber

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DEV_PatternCreation();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ClearDoodles();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _currentDoodler = Instantiate(DoodlerPrefab, this.transform); // instantiate as a child of the doodle master.
            _currentDoodler.DEV_PatternCreationDoodleMode(0.05f, 1000000);
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
        }
    }
    public void EndDoodlin()
    {
        CurrentDoodleOpenDistance = _currentDoodler.CalculateOpenDistance();
        DrawingOutput.AddRange(_currentDoodler.PrintPointOutput(0));
        _currentDoodler.SetLineColor(Color.cyan); //optional color change
        _currentDoodler = null;
    }
    public void SetALLDOODLESColor(Color targetColor, float targetAlpha)
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
    void DEV_PatternCreation()
    {
        if (!DEV_PATTERN_CREATION_MODE) return;
        if(DEV_PatternScriptableObject == null)
        {
            Debug.Log("ERROR null forgot to attach scriptable");
            return;
        }
        // USED for transfering pattern to a list scriptable Object
        List<Vector2> tempoutputlist = new List<Vector2>();
        foreach (Vector3 datapoint in DrawingOutput)
        {
            if (datapoint.z == 0)
            {
                tempoutputlist.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
        DEV_PatternScriptableObject.ReferenceShapeData = tempoutputlist;
#if UNITY_EDITOR
        LetsGetDirty(DEV_PatternScriptableObject);
        // Saves data written to scriptable objects through code.
        void LetsGetDirty(UnityEngine.Object scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}
