using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Search : MonoBehaviour
{
    [SerializeField]
    private MoveEnemy2 moveEnemy2;
    [SerializeField]
    private SphereCollider searchArea;

    private bool hasTriggered = false;

    private void OnTriggerStay(Collider other)
    {
        if (hasTriggered)
        {
            if (other.tag == "Player")
            {
                moveEnemy2.SetState(MoveEnemy2.EnemyState.Chase, other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hasTriggered = false;
            moveEnemy2.SetState(MoveEnemy2.EnemyState.Wait);
        }
    }

    public void HasTrigger()
    {
        hasTriggered = true;
    }
}
