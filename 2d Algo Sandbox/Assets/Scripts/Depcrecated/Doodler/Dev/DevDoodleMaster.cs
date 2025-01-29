using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevDoodleMaster : MonoBehaviour
{
    public DevDoodler DoodlerPrefab;
    DevDoodler _currentDoodler;
    int _DoodleSortOrder = 50;
    public List<Vector3> AccPointPositions;

    public GameObject AccPointPrefab;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _currentDoodler = Instantiate(DoodlerPrefab, this.transform); // instantiate as a child of the doodle master.
            _DoodleSortOrder += 1;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _currentDoodler.SetLineColor(Color.cyan, 0.5f); //and destroy edge collider
            AccPointPositions = _currentDoodler.GenerateAccuracyPoints();
            GameObject accpointParent = new GameObject();
            accpointParent.name = "AccPoints";
            foreach (Vector3 t in AccPointPositions)
            {
                Instantiate(AccPointPrefab, t, Quaternion.identity, accpointParent.transform);
            }

            _currentDoodler = null;
        }
        if (_currentDoodler != null)
        {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currentDoodler.UpdateLine(mousepos);

        }
    }
}
