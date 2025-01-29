using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodleMaster : MonoBehaviour
{
    public Doodler DoodlerPrefab;
    Doodler _currentDoodler;
    int _DoodleSortOrder = 50;

    [SerializeField] List<Doodler> _DoodlesList;
    void TEMPFUNCTIONS()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ClearDoodles();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetFinalColor(Color.red, 1);
        }
    }
    void Update()
    {
        TEMPFUNCTIONS();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _currentDoodler = Instantiate(DoodlerPrefab, this.transform); // instantiate as a child of the doodle master.
            _currentDoodler.SetSortingOrder(_DoodleSortOrder);
            _DoodleSortOrder += 1;
            _DoodlesList.Add(_currentDoodler); // adds all doodles to a list for easy clearing
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _currentDoodler.SetLineColor(Color.cyan, 0.5f); //and destroy edge collider
            _currentDoodler = null;
        }
        if (_currentDoodler != null)
        {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currentDoodler.UpdateLine(mousepos);
        }
    }
    public void SetFinalColor(Color targetColor, float targetAlpha)
    {
        // SUBSIGIL DRAWING DONE
        foreach (Doodler d in _DoodlesList)
        {
            d.SetLineColor(targetColor, targetAlpha);
        }
    }
    public void ClearDoodles()
    {
        print("CLEARING DOODLES!!!");
        foreach (Doodler d in _DoodlesList)
        {
            Destroy(d.gameObject);
        }
        _DoodleSortOrder = 50;
        _DoodlesList.Clear();
    }
}
