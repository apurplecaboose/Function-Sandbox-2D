using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class OCR_K_nearestneighbor : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    public List<Vector2> _DoodlePointOutputVector2;
    public List<PatternStorageObject> Patterns;
    public List<string> OUTPUTLIST;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            MatchBestPattern(_DoodlePointOutputVector2, Patterns);
        }
    }
    //if mode % is not high enough reject drawing. Ex: mode is 5(mode)/100(total count). this would be an example of a player drawing garbage needs further testing
    
    public string MatchBestPattern(List<Vector2> playerdrawingoutput, List<PatternStorageObject> patternlist)
    {
        string FindMode(List<string> outputnames)
        {
            var mode = outputnames.GroupBy(name => name)
                                  .OrderByDescending(group => group.Count())
                                  .ThenBy(group => group.Key)
                                  .Select(group => group.Key)
                                  .FirstOrDefault();
            return mode;
        }
        int CountOccurrences(List<string> stringsList, string targetString)
        {
            var count = stringsList.Count(item => item == targetString);
            return count;
        }

        List<string> Find_K_NearestNeighbor_OnePoint(Vector2 inputPoint, int k) // will output the names of the nearest neighbor reference pattern names
        {
            List<string> onepointoutput = new List<string>();
            List<KeyValuePair<float, string>> keypairlist = new List<KeyValuePair<float, string>>();
            foreach (var keypat in patternlist)
            {
                string currentpatternname = keypat.Pattern_Name;
                foreach (Vector2 p_point in keypat.ReferenceShapeData)
                {
                    float pointdist = Vector2.Distance(inputPoint, p_point);
                    keypairlist.Add(new KeyValuePair<float, string>(pointdist, currentpatternname));
                }
            }
            keypairlist = keypairlist.OrderBy(f => f.Key).ToList();
            for (int i = 0; i < k; i++)
            {
                onepointoutput.Add(keypairlist[i].Value);
            }
            return onepointoutput;
        }
        //
        List<string> KNN_list = new List<string>();
        foreach (Vector2 point in playerdrawingoutput)
        {
            KNN_list.AddRange(Find_K_NearestNeighbor_OnePoint(point, 5));
        }
        OUTPUTLIST = KNN_list;
        string bestmatchoutput = FindMode(KNN_list);
        int mode = CountOccurrences(KNN_list, bestmatchoutput);
        float KNNperct = mode / KNN_list.Count;
        print("Best Match: " + bestmatchoutput + "; Mode: " + mode + "; Total Count: " + KNN_list.Count + "; Percentage: " + KNNperct);
        return bestmatchoutput;
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
    }

}
