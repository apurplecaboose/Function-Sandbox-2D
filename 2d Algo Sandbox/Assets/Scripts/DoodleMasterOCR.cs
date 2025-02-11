using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoodleMasterOCR : MonoBehaviour
{
    [Header("ATTACH")]
    public DoodleOCR DoodlerPrefab;

    [Header("DEV SETTINGS")]
    [SerializeField] int _devIndex = 0;
    public bool DEV_PATTERN_CREATION_MODE;
    public List<RawShapes> RawTrainingData;
    List<ShapeGroup> shapegroups;

    [Header ("Attributes")]
    DoodleOCR _currentDoodler;
    /*[SerializeField]*/ List<DoodleOCR> _DoodlesList; // currently serialized for visibility
    [HideInInspector] public List<Vector3> DrawingOutput; // x,y,doodlenumber

    void Awake()
    {
        _DoodlesList = new List<DoodleOCR>();
    }
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
            _currentDoodler.parentDoodler = this;
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
        DrawingOutput.AddRange(_currentDoodler.ExportCleanPointCloud(0));
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
        if(RawTrainingData.Count == 0)
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
#if UNITY_EDITOR
        if (RawTrainingData != null)
        {
            if (_devIndex >= RawTrainingData.Count) { Debug.Log("DONE RECORDING!!!!!!!!!!!!!!"); return; }
            RawTrainingData[_devIndex].RawVector2DataPoints = tempoutputlist;
            HELPER_FUNCS.LetsGetDirty(RawTrainingData[_devIndex]);
            print("Getting Dirty");
        }
#endif
        //increment next drawing for training data
        _devIndex += 1;
        ClearDoodles();
    }
}
