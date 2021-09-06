using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollisionVerfication : MonoBehaviour
{
    [SerializeField] private SnapSystem snapTriggerSystem;
    [SerializeField] private CollisionDetecter collisionDetecter;

    private bool collisionDetectedByGhostprefab;

    public bool CollisionDetectedByGhostprefab { get => collisionDetectedByGhostprefab; set => collisionDetectedByGhostprefab = value; }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CollisionDetecter>() && other.GetComponent<CollisionDetecter>() != collisionDetecter)
        {
                collisionDetectedByGhostprefab = true;
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
