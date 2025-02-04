using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(ShapeStorage))]
public class Trainer : MonoBehaviour
{
    public ShapeStorage TrainingTarget;
    public List<Vector2> KeyPoints; //top left to bottom right; left to right; top to bottom; 

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) EXECUTE_the_Training();
    }
    void EXECUTE_the_Training()
    {
        for (int i = 0; i < KeyPoints.Count; i++)
        {
            List<float> rawsigs = RawOnePoint_MeanSquareDistance(KeyPoints[i]);

            switch (i)
            {
                case 0:
                    rawsigs.AddRange(TrainingTarget.raw_sig_top);
                    rawsigs.AddRange(TrainingTarget.raw_sig_left);
                    break;
                case 1:
                    rawsigs.AddRange(TrainingTarget.raw_sig_top);
                    break;
                case 2:
                    rawsigs.AddRange(TrainingTarget.raw_sig_top);
                    rawsigs.AddRange(TrainingTarget.raw_sig_right);
                    break;
                case 3:
                    rawsigs.AddRange(TrainingTarget.raw_sig_left);
                    break;
                case 4:
                    rawsigs.AddRange(TrainingTarget.raw_middleweight); //middle
                    break;
                case 5:
                    rawsigs.AddRange(TrainingTarget.raw_sig_right);
                    break;
                case 6:
                    rawsigs.AddRange(TrainingTarget.raw_sig_bottom);
                    rawsigs.AddRange(TrainingTarget.raw_sig_left);
                    break;
                case 7:
                    rawsigs.AddRange(TrainingTarget.raw_sig_bottom);
                    break;
                case 8:
                    rawsigs.AddRange(TrainingTarget.raw_sig_bottom);
                    rawsigs.AddRange(TrainingTarget.raw_sig_right);
                    break;
            }
        }

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

        print("DONE");
    }


    List<float> RawOnePoint_MeanSquareDistance(Vector2 keypoint) // will output the names of the nearest neighbor reference pattern names
    {
        List<float> outputlist = new List<float>();
        foreach (var raw in TrainingTarget.RawData)
        {
            List<float> meansquareDistances = new List<float>();
            foreach (Vector2 vec in raw.Rawvec2data)
            {
                float squareDistance = Mathf.Pow(Vector2.Distance(vec, keypoint), 2); // squared distance to increase error
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
