using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
//[RequireComponent(typeof(ShapeStorage))]
public class Trainer : MonoBehaviour
{
    Stopwatch _benchmark;//FOR TESTING ONLY
    [SerializeField] float _Keypointscale = 75000;
    public ShapeStorage TrainingTarget;
    [SerializeField]List<Vector2> _KeyPoints; //top left to bottom right; left to right; top to bottom; 
    void Awake()
    {
        if(_KeyPoints != null) _KeyPoints.Clear();

        _KeyPoints = new List<Vector2> { new Vector2 (-1,1), new Vector2 (0,1), new Vector2 (1,1),
                                        new Vector2 (-1,0), new Vector2 (0,0), new Vector2 (1,0),
                                        new Vector2 (-1,1), new Vector2 (-1,0), new Vector2 (-1,1)};
        // scale keypoints to new arbitrary scale
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            _KeyPoints[i] = new Vector2(_KeyPoints[i].x * _Keypointscale, _KeyPoints[i].y * _Keypointscale);
        }
        _benchmark = new Stopwatch();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("start training");
            ClearPreviousCalculations();
            _benchmark.Start();
            EXECUTE_the_Training();
        }
    }
    void ClearPreviousCalculations()
    {
        TrainingTarget.raw_middleweight.Clear();
        TrainingTarget.raw_sig_bottom.Clear();
        TrainingTarget.raw_sig_top.Clear();
        TrainingTarget.raw_sig_left.Clear();
        TrainingTarget.raw_sig_right.Clear();
    }
    void EXECUTE_the_Training()
    {
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            List<float> rawsigs = new List<float>();
            rawsigs.AddRange(RawOnePoint_MeanSquareDistance(_KeyPoints[i]));

            switch (i)
            {
                case 0:
                    TrainingTarget.raw_sig_top.AddRange(rawsigs);
                    TrainingTarget.raw_sig_left.AddRange(rawsigs);
                    break;
                case 1:
                    TrainingTarget.raw_sig_top.AddRange(rawsigs);
                    break;
                case 2:
                    TrainingTarget.raw_sig_top.AddRange(rawsigs);
                    TrainingTarget.raw_sig_right.AddRange(rawsigs);
                    break;
                case 3:
                    TrainingTarget.raw_sig_left.AddRange(rawsigs);
                    break;
                case 4:
                    TrainingTarget.raw_middleweight.AddRange(rawsigs); //middle
                    break;
                case 5:
                    TrainingTarget.raw_sig_right.AddRange(rawsigs);
                    break;
                case 6:
                    TrainingTarget.raw_sig_bottom.AddRange(rawsigs);
                    TrainingTarget.raw_sig_left.AddRange(rawsigs);
                    break;
                case 7:
                    TrainingTarget.raw_sig_bottom.AddRange(rawsigs);
                    break;
                case 8:
                    TrainingTarget.raw_sig_bottom.AddRange(rawsigs);
                    TrainingTarget.raw_sig_right.AddRange(rawsigs);
                    break;
            }
        }
        FillData();
        //Invoke("FillData", 2);

    }

    void FillData()
    {
        Vector2 top = CalculateMeanAndStdDev(TrainingTarget.raw_sig_top);
        TrainingTarget.mean_top = top.x;
        TrainingTarget.std_top = top.y;
        Vector2 bot = CalculateMeanAndStdDev(TrainingTarget.raw_sig_bottom);
        TrainingTarget.mean_bottom = bot.x;
        TrainingTarget.std_bottom = bot.y;
        Vector2 left = CalculateMeanAndStdDev(TrainingTarget.raw_sig_left);
        TrainingTarget.mean_left = left.x;
        TrainingTarget.std_left = left.y;
        Vector2 right = CalculateMeanAndStdDev(TrainingTarget.raw_sig_right);
        TrainingTarget.mean_right = right.x;
        TrainingTarget.std_right = right.y;
        Vector2 middle = CalculateMeanAndStdDev(TrainingTarget.raw_middleweight);
        TrainingTarget.mean_middle =middle.x;
        TrainingTarget.std_middle = middle.y;   

        print("DONE");
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("time in ms: " + elapsedMilliseconds);
    }
    List<float> RawOnePoint_MeanSquareDistance(Vector2 keypoint) // will output the names of the nearest neighbor reference pattern names
    {
        List<float> outputlist = new List<float>();
        List<float> meansquareDistances = new List<float>();
        foreach (var raw in TrainingTarget.RawData)
        {
            foreach (Vector2 vec in raw.Rawvec2data)
            {
                float distancetokeypoint = Vector2.Distance(vec, keypoint);
                distancetokeypoint /= 1000;// reduce the scale of the numbers to something fathomable
                float squareDistance = Mathf.Pow(distancetokeypoint, 2); // squared distance to increase error
                meansquareDistances.Add(squareDistance);
            }
            outputlist.Add(meansquareDistances.Average());
        }
        return outputlist;
    }

    public Vector2 CalculateMeanAndStdDev(List<float> data)
    {
        // Calculate mean
        float mean = data.Average();

        // Calculate standard deviation
        float sumOfSquaresOfDifferences = data.Select(val => (val - mean) * (val - mean)).Sum();
        float stdDev = (float)Math.Sqrt(sumOfSquaresOfDifferences / data.Count);

        return new Vector2 (mean, stdDev);
    }
}
