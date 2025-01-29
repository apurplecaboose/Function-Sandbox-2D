using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyPoint : MonoBehaviour
{
    [SerializeField] Acc_n_Failure _AccMaster;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Doodle"))
        {
            _AccMaster.AccuracyIndex += 1;
            Destroy(gameObject);
        }
    }
}
