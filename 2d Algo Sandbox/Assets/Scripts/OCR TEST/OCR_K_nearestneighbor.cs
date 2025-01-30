using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class OCR_K_nearestneighbor : MonoBehaviour
{
    Stopwatch _benchmark;//FOR TESTING ONLY
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    public List<Vector2> _DoodlePointOutputVector2;
    public List<PatternStorageObject> Patterns;
    List<PatternStorageObject> _OpenPatterns, _ClosedPatterns;
    public List<string> OUTPUTLIST;
    void Awake ()
    {
        _benchmark = new Stopwatch();
        _OpenPatterns = new List<PatternStorageObject>();
        _ClosedPatterns = new List<PatternStorageObject>();
        foreach (var keypattern in Patterns)
        {
            if(keypattern.IsOpenShape) _OpenPatterns.Add(keypattern);
            else _ClosedPatterns.Add(keypattern);
        }
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartMatchingProcedure();
        }
    }
    void StartMatchingProcedure()
    {
        _benchmark.Start();
        //testing shape openess
        float closedthresholdvalue = 0.5f;
        if (DOODLEMASTERCOMPONENT.CurrentDoodleOpenDistance < closedthresholdvalue)
        {
            MatchBestPattern(_DoodlePointOutputVector2, _ClosedPatterns);
            //print("CLOSED Shape");
        }
        else
        {
            MatchBestPattern(_DoodlePointOutputVector2, _OpenPatterns);
            //print("OPEN Shape");
        }
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("time in ms: " + elapsedMilliseconds);
        _benchmark.Reset();
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
            //GPT says to speed up I can use miniheap only store values into a miniheap if they are the k number of smallest items then i can avoid the sort function
            //or heap into miniheap. When miniheap exceeds certian size remove max values.
            //EXAMPLE: if (minHeap.Count > k)minHeap.Remove(minHeap.Max);
            foreach (var keypat in patternlist)
            {
                string currentpatternname = keypat.Pattern_Name;
                foreach (Vector2 p_point in keypat.ReferenceShapeData)
                {
                    float pointdist = Vector2.Distance(inputPoint, p_point);

                    keypairlist.Add(new KeyValuePair<float, string>(pointdist, currentpatternname));
                    if (keypairlist.Count > k)
                    {
                        KeyValuePair<float, string> maxKeyValuePair = keypairlist[0];
                        float maxFloat = maxKeyValuePair.Key;
                        // Find the KeyValuePair with the maximum float value in a single pass
                        for (int i = 1; i < keypairlist.Count; i++)
                        {
                            if (keypairlist[i].Key > maxFloat)
                            {
                                maxKeyValuePair = keypairlist[i];
                                maxFloat = keypairlist[i].Key;
                            }
                        }
                        keypairlist.Remove(maxKeyValuePair);
                    }
                }
            }
            keypairlist = keypairlist.OrderBy(f => f.Key).ToList(); // not needed anymore if using miniheap... potentially?
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
            KNN_list.AddRange(Find_K_NearestNeighbor_OnePoint(point, 3));
        }
        OUTPUTLIST = KNN_list;
        string bestmatchoutput = FindMode(KNN_list);
        int mode = CountOccurrences(KNN_list, bestmatchoutput);
        float KNNperct = (float)mode / (float)KNN_list.Count;
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
