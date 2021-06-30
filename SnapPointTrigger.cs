using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnapPointTrigger : MonoBehaviour
{
    [SerializeField] private SnapPointTriggerSystem snapTriggerSystem;
    [SerializeField] private CollisionDetecter collisionDetecter;
    
    private BoxCollider snapCollider;
    public BoxCollider SnapCollider => snapCollider;
    private bool isDetectedOtherSnapPoint;

    public SnapPointTrigger currentSnapPointContainer;
    public SnapPointTrigger snapPointBucket;
    
    private bool checkingSnapPoints;

    private bool SnapPointDisabled;


    private void Awake()
    {
        SnapPointDisabled = false;
        snapCollider = GetComponent<BoxCollider>();
       // EventBus.Instance.OnGameObjectDrag += OnGameObjectDrag;
    }


    // DISABLE SnapPointsTriggers And Attach (AND the rest of SELECTED GO's SnapPoints IF
    // OTHER SnapPoints is SAME Position AS OTHER DETECTED SnapPoints)

    // DISABLE Collider Detection on THIS SnapPointTrigger after Snap
    public void DisableSnapPoints()
    {
        EventBus.Instance.OnGameObjectDrag += EnableSnapPointsAndDetachSnapPoints;
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
        EventBus.Instance.OnGameObjectDrag -= EnableSnapPointsAndDetachSnapPoints;
        EventBus.Instance.OnSelectGO -= SelectGo;
    }


    // Perhaps use later (SnapPoint Attachment Method)

    //private IEnumerator AttachOtherSnapPoints()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    foreach (SnapPointTrigger snapPointTrigger in transform.parent.GetComponentsInChildren<SnapPointTrigger>())
    //    {
    //        if (snapPointTrigger.snapPointBucket)
    //        {

    //          //  snapPointTrigger.currentSnapPointContainer.currentSnapPointContainer = snapPointTrigger.snapPointBucket;
    //           // snapPointTrigger.currentSnapPointContainer.DisableSnapPoints();
    //            snapPointTrigger.currentSnapPointContainer = snapPointTrigger.snapPointBucket;
    //         //   snapPointTrigger.DisableSnapPoints();
    //            //   snapPointTrigger.DisableSnapPoints();
    //        }

    //        // SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.DisableSnapPoints();
    //    }
    //    yield return new WaitForSeconds(0.2f);
    //        DisableSnapPoints();

    //}

    private void EnableSnapPointsAndDetachSnapPoints(Vector3 selectedGo)
    {
        EventBus.Instance.OnGameObjectDrag -= EnableSnapPointsAndDetachSnapPoints;

        if (SelectionHandler.SelectedGO == snapTriggerSystem.gameObject && SnapPointDisabled)
        {
            // this.currentSnapPointContainer.currentSnapPointContainer.ToggleSnapCollider(true);
            currentSnapPointContainer.currentSnapPointContainer = null;     // Detach OTHER Go SnapPoint First
            currentSnapPointContainer = null;                              // Detach THIS Go SnapPoint Second

            SnapPointDisabled = false;
            Debug.Log("SnapPoint " + transform.name + " and SnapPoint " + SnapPointDetectionController.Instance.currentDETECTEDSnapPoint + "Is ENABLED");
        }
    }

    // Will be Triggered If Any Colliders is Detected
    private void OnTriggerStay(Collider other)
    {
        if (snapTriggerSystem.gameObject == SelectionHandler.SelectedGO) // IF THIS Parent GameObject is The selected Product
        {
            ToggleSnapPointCollider(true);
               // SnapPointDisabled = false;
            if (collisionDetecter.CollisionDetected)
            {
                snapTriggerSystem.ToggleGhostHighlighter();
                EmptyCurrentSelectedSnapPoints();
                return;
            }

            else if (other.GetComponent<SnapPointTrigger>())
            {

                Debug.Log("Snap Point Detected");
                if (SnapPointDetectionController.Instance.currentSELECTEDSnapPoint != GetComponent<SnapPointTrigger>())
                {
                    if (currentSnapPointContainer == null && other.GetComponent<SnapPointTrigger>().currentSnapPointContainer == null)
                    {
                        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint = GetComponent<SnapPointTrigger>();
                        SnapPointDetectionController.Instance.currentDETECTEDSnapPoint = other.GetComponent<SnapPointTrigger>();
                           snapTriggerSystem.ToggleGhostHighlighter();
                        
                    }
                    else
                    {
                        EmptyCurrentSelectedSnapPoints();
                        snapTriggerSystem.ToggleGhostHighlighter();
                    }
                }
            }
        }
       // Will Turn Off Collider If The SnapPoint Trigger is Inside Another Collider
        else if (other.GetComponent<SnapPointTriggerSystem>() && other.GetComponent<SnapPointTriggerSystem>() != snapTriggerSystem && other.GetComponent<SnapPointTrigger>() != GetComponent<SnapPointTrigger>())
        {
            if (SelectionHandler.SelectedGO != null && other.GetComponent<SnapPointTriggerSystem>() != SelectionHandler.SelectedGO.GetComponent<SnapPointTriggerSystem>())
            {
                EventBus.Instance.OnSelectGO += SelectGo; 
                ToggleSnapPointCollider(false);
                Debug.Log("Detected other Collider!");
            }
        }
    }

    private void SelectGo(GameObject obj)
    {
            Debug.Log("TESTTESTTEST");
            ToggleSnapPointCollider(true);
        EventBus.Instance.OnSelectGO -= SelectGo;
        snapTriggerSystem.ToggleGhostHighlighter();
    }

    private void ToggleSnapPointCollider(bool toogle)
    {
        this.snapCollider.enabled = toogle;
    }

    private static void EmptyCurrentSelectedSnapPoints()
    {
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint = null;
        SnapPointDetectionController.Instance.currentDETECTEDSnapPoint = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (snapTriggerSystem.gameObject == SelectionHandler.SelectedGO && other.GetComponent<SnapPointTrigger>())
        {
            EmptyCurrentSelectedSnapPoints();
            EmptySnapPointBucket(other);
            snapTriggerSystem.ToggleGhostHighlighter();
        }
    }

    private void EmptySnapPointBucket(Collider other)
    {
        other.GetComponent<SnapPointTrigger>().currentSnapPointContainer = null;
        currentSnapPointContainer = null;
        snapPointBucket = null;
        snapTriggerSystem.ToggleGhostHighlighter();
    }
}
