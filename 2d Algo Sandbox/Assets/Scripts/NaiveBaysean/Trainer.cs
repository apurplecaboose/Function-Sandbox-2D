using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class Trainer : MonoBehaviour
{
    [SerializeField] float _Keypointscale = 30000;
    [SerializeField] int _gridsize = 11;
    public Cooked_Shape TrainingTarget;
    public List<RawTrainingData> TrainingData;

    Stopwatch _benchmark;//FOR TESTING ONLY
    List<Vector2> _KeyPoints; //top left to bottom right; left to right; top to bottom; 

    List<List<float>> _rawsigmaNested;
    void Awake()
    {
        _rawsigmaNested = new List<List<float>>();
        _KeyPoints = new List<Vector2>();
        var grid = HELPER_FUNCS.GenerateThenScaleGridPoints(_gridsize,_Keypointscale);
        _KeyPoints = grid.Item1;

        if(TrainingTarget == null) UnityEngine.Debug.LogError("No Target!!!");
        if (TrainingData.Count == 0) UnityEngine.Debug.LogError("No Training Data!!!");
        _benchmark = new Stopwatch();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("start training");
            _benchmark.Start();
            EXECUTE_the_Training();
        }
    }
    void EXECUTE_the_Training()
    {
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            List<float> rawsigmas = new List<float>();
            rawsigmas.AddRange(RawOnePoint_MeanSquareDistance(_KeyPoints[i]));
            _rawsigmaNested.Add(rawsigmas);
        }
        FillData();
    }

    void FillData()
    {
        List<float> meanoutput = new List<float>();
        List<float> stdoutput = new List<float>();
        for (int i = 0; i < _KeyPoints.Count; i++)
        {        
            meanoutput.Add(_rawsigmaNested[i].Average());
            stdoutput.Add(HELPER_FUNCS.CalculateStdDev(_rawsigmaNested[i]));
        }
        TrainingTarget.Mean_Point_Weights = meanoutput;
        TrainingTarget.STD_Point_Weights = stdoutput;

#if UNITY_EDITOR
        HELPER_FUNCS.LetsGetDirty(TrainingTarget);
#endif
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("DONE Training Data! Time in ms: " + elapsedMilliseconds);
    }
    List<float> RawOnePoint_MeanSquareDistance(Vector2 keypoint) // will output the names of the nearest neighbor reference pattern names
    {
        List<float> outputlist = new List<float>();
        List<float> meansquareDistances = new List<float>();
        foreach (var raw in TrainingData)
        {
            foreach (Vector2 v in raw.RawVector2DataPoints)
            {
                float distancetokeypoint = Vector2.Distance(v, keypoint);
                distancetokeypoint /= 1000;// reduce the scale of the numbers to something fathomable
                float squareDistance = Mathf.Pow(distancetokeypoint, 2); // squared distance to increase error
                meansquareDistances.Add(squareDistance);
            }
            outputlist.Add(meansquareDistances.Average());
        }
        return outputlist;
    }
}
