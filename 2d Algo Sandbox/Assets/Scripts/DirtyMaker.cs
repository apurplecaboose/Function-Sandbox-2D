using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyMaker : MonoBehaviour
{
    [Header("Dirty-fies everything in the list on start")]
    public bool CommenceDirtification;
    public List<Object> ListOfCleanThingsThatShouldBeDirty;
    void Start()
    {
        if(!CommenceDirtification) return;
        foreach (var v in ListOfCleanThingsThatShouldBeDirty)
        {
           HELPER_FUNCS.LetsGetDirty(v);
        }
    }
}
