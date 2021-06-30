using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostVerifySnapPosition : MonoBehaviour
{
    [SerializeField] private SnapPointTriggerSystem snapTriggerSystem;
    [SerializeField] private CollisionDetecter collisionDetecter;
    private  bool collisionDetectedByGhostprefab;

    public bool CollisionDetectedByGhostprefab { get => collisionDetectedByGhostprefab;}

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CollisionDetecter>() && other.GetComponent<CollisionDetecter>() != collisionDetecter)
        {
            collisionDetectedByGhostprefab = true;
            transform.parent.GetChild(0).gameObject.SetActive(false);
            Debug.Log("Ghost Prefab Collision Detection" + collisionDetectedByGhostprefab);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CollisionDetecter>() && other.GetComponent<CollisionDetecter>() != collisionDetecter)
        {
            collisionDetectedByGhostprefab = false;
            Debug.Log("Ghost Prefab Collision Detection" + collisionDetectedByGhostprefab);
        }
    }
}
