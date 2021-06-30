using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour
{
    [SerializeField] ProductID productId;
    [SerializeField] GameObject arrowGo;
    [SerializeField] GameObject crossWarningCollisionGo;
    [SerializeField]  MeshRenderer highLighterCollision;
    private static bool collisionDetected;
    public bool CollisionDetected { get => collisionDetected; }

    private void Awake()
    {
        crossWarningCollisionGo.SetActive(false);
        if (highLighterCollision.enabled)
            highLighterCollision.enabled = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (productId.gameObject != other.gameObject && other.GetComponent<CollisionDetecter>())
        {
            if (!collisionDetected)
            {
                    Debug.Log("Collision Detected" + productId.transform);
                ToggleCollisionMeshHighlighter(other, true);
            }
        }    
    }


    private void OnTriggerExit(Collider other)
    {
        ToggleCollisionMeshHighlighter(other, false);
    }
    public void ToggleCollisionMeshHighlighter(Collider other, bool detected)
    {
        if (productId.gameObject == SelectionHandler.SelectedGO)
        {
            if (other.GetComponent<CollisionDetecter>() && other.GetComponent<ProductID>() != productId)
            {
                collisionDetected = detected;
                highLighterCollision.enabled = detected;
                arrowGo.SetActive(!detected);
                crossWarningCollisionGo.SetActive(detected);
            }
        }
    }
    public void ToggleCollisionGhostMeshHighlighter(Collider other, bool detected)
    {
                collisionDetected = detected;
                highLighterCollision.enabled = detected;
    }

}
