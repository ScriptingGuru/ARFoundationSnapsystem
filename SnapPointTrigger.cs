using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Other snappoints are detected by this script using OnTriggerStay Event.
/// If two snapPoints are detected, THIS SnapPointTrigger and another SnapPointTrigger script, will
/// initiate the snap process <see cref="SnapSystem"/>.
/// </summary>

public class SnapPointTrigger : MonoBehaviour
{
    [SerializeField] private SnapSystem snapTriggerSystem;
    [SerializeField] private CollisionDetecter collisionDetecter;
    
    private BoxCollider snapCollider;

    [ReadOnly] public SnapPointTrigger currentDetectedSnapPoint;
    private SnapPointTrigger thisSnapPointTrigger;
    private bool SnapPointDisabled;
    private const float minAngle = 150f;

    private void Awake()
    {
        thisSnapPointTrigger = GetComponent<SnapPointTrigger>();
         SnapPointDisabled = false;
        snapCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        if (SelectionHandler.SelectedGO != snapTriggerSystem.gameObject)
        {
            ToggleSnapPointCollider(true);
        }
    }

    private void OnDestroy()
    {
       // EventBus.Instance.OnSelectGO -= EnableSnapPointsAndDetachSnapPoints;
        EventBus.Instance.OnSelectGO -= OnSelectedGameObject;
    }


    /// <summary>
    /// THIS "ONTRIGGERSTAY" FLOW DETECTS OTHER SNAPPOINT
    /// TRIGGERS UPON WHICH TO SNAP. IT ALSO DETECTS OTHER
    /// COLLIDERS IN ORDER TO TURN ITSELF OFF IF THIS GAMEOBJECT (PARENT)
    /// ISN'T THE ONE SELECTED.
    /// </summary>

    private void OnTriggerStay(Collider other)
    {
        // If this is the selected gameobject...
        if (snapTriggerSystem.gameObject == SelectionHandler.SelectedGO)
        {
            // Activate Snappoint trigger if disabled...
            ToggleSnapPointCollider(true);
            // Abort If this gameobject collides with anything...
            if (StaticCollisionParameters.CollisionDetected)
            {
                EmptyCurrentSelectedSnapPoints();
                return;
            }

            // If this snappoint trigger isn't colliding with anything and another snappoint is detected,
            else if (other.GetComponent<SnapPointTrigger>())
            {
                float angleBetweenVectors = Vector3.Angle(other.GetComponent<SnapPointTrigger>().transform.forward, thisSnapPointTrigger.transform.forward);


                    // Is my snappoint direction equivalent to that of another snappoint?
                    if (angleBetweenVectors > minAngle) // they're in roughly the same direction
                    {
                    SnapPointTrigger currentDetectedSnapPointTrigger = other.GetComponent<SnapPointTrigger>();
                    float distance = Vector3.Distance(transform.position, currentDetectedSnapPointTrigger.transform.position);

                    // If two or more snappoints are detected at the same time from the same Gameobject, determine which
                    // snappoint has the closest distance from the selected snappoint to the detected snappoint.
                    // The one with the shortest distance becomes the new Current Selected SnapPoint

                   // Debug.Log("Other SnapPoint Distance: " + distance + "   " + " Current Closest Distance: " + SnapPointDetectionHandler.ClosestDistance);
                    if (SnapPointDetectionHandler.currentSELECTEDSnapPoint != null && SnapPointDetectionHandler.currentSELECTEDSnapPoint != thisSnapPointTrigger)
                    {
                        if (distance < SnapPointDetectionHandler.ClosestDistance)
                        {
                            SnapPointDetectionHandler.ClosestDistance = distance;
                            AddSnapPoints(currentDetectedSnapPointTrigger, thisSnapPointTrigger);
                            snapTriggerSystem.InitalizeSnapProcess(); // SnapPoint Ready To Snap
                        }
                    }
                    else
                    {
                        SnapPointDetectionHandler.ClosestDistance = distance;
                        AddSnapPoints(currentDetectedSnapPointTrigger, thisSnapPointTrigger);
                        snapTriggerSystem.InitalizeSnapProcess(); // SnapPoint Ready To Snap
                    }
                }
            }
        }

        // If SnapPoint is not the selected gameobject
        // and the SnapPoint Trigger is located within
        // another collider, the collider will be disabled.
        else if (other.GetComponent<SnapSystem>() && other.GetComponent<SnapSystem>() != snapTriggerSystem)
        {
            if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO.GetComponent<SnapSystem>() != other.GetComponent<SnapSystem>())
            {
                EventBus.Instance.OnSelectGO += OnSelectedGameObject;
                ToggleSnapPointCollider(false);
            }
        }
    }

    private static void AddSnapPoints(SnapPointTrigger currentDetectedSnapPointTrigger, SnapPointTrigger thisCurrentSnapPointTrigger)
    {
        if (SnapPointDetectionHandler.currentSELECTEDSnapPoint != thisCurrentSnapPointTrigger)
            SnapPointDetectionHandler.currentSELECTEDSnapPoint = thisCurrentSnapPointTrigger;
     
        if (SnapPointDetectionHandler.currentDETECTEDSnapPoint != currentDetectedSnapPointTrigger)
            SnapPointDetectionHandler.currentDETECTEDSnapPoint = currentDetectedSnapPointTrigger;
    }

    private void OnSelectedGameObject(GameObject obj, ProductPrefabDataManager productPrefabDataManager)
    {
        ToggleSnapPointCollider(true);
        EventBus.Instance.OnSelectGO -= OnSelectedGameObject;
    }

    private void ToggleSnapPointCollider(bool toogle)
    {
        if (snapCollider.enabled != toogle)
        {
            this.snapCollider.enabled = toogle;
        }
    }

    private static void EmptyCurrentSelectedSnapPoints()
    {
        SnapPointDetectionHandler.currentSELECTEDSnapPoint = null;
        SnapPointDetectionHandler.currentDETECTEDSnapPoint = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (snapTriggerSystem.gameObject == SelectionHandler.SelectedGO && other.GetComponent<SnapPointTrigger>())
        {
            if (other.GetComponent<SnapPointTrigger>() == SnapPointDetectionHandler.currentDETECTEDSnapPoint)
            {
                EmptyCurrentSelectedSnapPoints();
                snapTriggerSystem.ResetSnapProcess();
            }
        }
    }
}