using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class will combine two Snappable items together.
/// The CURRENT SELECTED Go AND The Detected SnapPoint Go
/// This occurs when the finger has first touched, dragged, and then been released off the screen.
/// </summary>
/// 
public class SnapPointTriggerSystem : MonoBehaviour
{
    [SerializeField] private GameObject snapPointsParent;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private CollisionDetecter collisionDetecter;
    [SerializeField] private GhostVerifySnapPosition ghostCollider;
    [SerializeField] private ArrowHighlighter arrowPrefab;
    private Transform lastGhostPosition;

    private Transform ParentObject;
    bool isDraggingGO;

    private void Awake()
    {
        if (ghostPrefab)
            ghostPrefab.transform.GetChild(0).gameObject.SetActive(false);
        snapPointsParent.SetActive(false);
        collisionDetecter.gameObject.SetActive(false);
           ParentObject = GetComponent<Transform>();
        EventBus.Instance.OnDragStart += OnDragStart;
        EventBus.Instance.OnDragEnd += OnDragEnd;
        EventBus.Instance.OnSelectGO += OnSelectGameObject;
        EventBus.Instance.OnDrag += OnDrag;
        EventBus.Instance.OnGameObjectDrag += Instance_OnGameObjectDrag;
    }

    private void OnSelectGameObject(GameObject go)
    {
        ghostCollider.GetComponent<Collider>().enabled = false;
    }

    private void Start()
    {
        ghostPrefab.transform.parent = this.transform.parent;
    }

    private void OnDrag(Vector2 touchPos)
    {
           // ToggleGhostHighlighter();
        if (!isDraggingGO)
        {
            isDraggingGO = true;
            snapPointsParent.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        EventBus.Instance.OnDragStart -= OnDragStart;
        EventBus.Instance.OnDragEnd -= OnDragEnd;
        EventBus.Instance.OnDrag -= OnDrag;
        EventBus.Instance.OnGameObjectDrag -= Instance_OnGameObjectDrag;
        EventBus.Instance.OnSelectGO -= OnSelectGameObject;
    }

    public void ToggleGhostHighlighter()
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {

            if (SnapPointDetectionController.Instance.currentSELECTEDSnapPoint != null && SnapPointDetectionController.Instance.currentDETECTEDSnapPoint != null)
            {
                ghostCollider.GetComponent<Collider>().enabled = true;
                GhostSnapper();
            }
            else
            {
                ghostCollider.GetComponent<Collider>().enabled = false;
                ghostPrefab.transform.GetChild(0).gameObject.SetActive(false);
                arrowPrefab.EnableArrowDefaultColor();
            }
        }
    }

    private void Instance_OnGameObjectDrag(Vector3 worldPosition)
    {
        if (!isDraggingGO)
        {
            isDraggingGO = true;
            collisionDetecter.gameObject.SetActive(true);
            snapPointsParent.SetActive(true);
        }
    }


    public void OnDragStart(Vector2 vector2)
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {
            snapPointsParent.SetActive(true);
        }
    }

    public void OnDragEnd(Vector2 vector2)
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {
            if (isDraggingGO)
            {
                isDraggingGO = false;
                ToggleGhostHighlighter();
                if (NoSnapPointsToAttach())
                {
                    snapPointsParent.SetActive(false);
                    arrowPrefab.EnableArrowDefaultColor();
                    ghostCollider.GetComponent<Collider>().enabled = false;
                }

                else
                {
                    if (!ghostCollider.CollisionDetectedByGhostprefab)
                    {
                        snapPointsParent.SetActive(true);
                        ResetEventsBeforeSnap();
                    }
                }
                
            }
        }
    }

    private bool NoSnapPointsToAttach()
    {
        bool value;
        
        if (!SnapPointDetectionController.Instance.currentSELECTEDSnapPoint || !SnapPointDetectionController.Instance.currentDETECTEDSnapPoint)
        {
            value = true;
        }
        else
        {
            value = false;
        }
        return value;
    }

    private void ResetEventsBeforeSnap()
    {
        SelectionHandler.latestSelectedGO = SelectionHandler.SelectedGO;
        ghostPrefab.transform.GetChild(0).gameObject.SetActive(false);
        GameObject tempGo = SelectionHandler.SelectedGO;
        SelectionHandler.SelectedGO = null;
        EventBus.Instance.DeselectGO(tempGo);
        Snap(ParentObject);
    }


    // Move the GhostHighLighter to correct Detected SnapPoint Position
    private void GhostSnapper()
    {
        ghostPrefab.transform.GetChild(0).gameObject.SetActive(false);
        ghostPrefab.transform.position = this.transform.position;
        ghostPrefab.transform.rotation = this.transform.rotation;
        ghostPrefab.transform.position = SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.transform.position;
        ghostPrefab.transform.rotation = SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.transform.rotation;
        ghostPrefab.transform.localRotation *= Quaternion.Euler(0, -180, 0);
        lastGhostPosition = ghostPrefab.transform;
       
        if (ghostCollider.CollisionDetectedByGhostprefab)
        {
            ghostPrefab.transform.GetChild(0).gameObject.SetActive(false);
            arrowPrefab.EnableArrowDefaultColor();
        }
        else
        {
            ghostPrefab.transform.GetChild(0).gameObject.SetActive(true);
            arrowPrefab.EnableArrowSnapColor();
        }

    }


    // Move the product to correct SnapPoint Position
    private void Snap(Transform parent)
    {
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.parent = this.transform.parent;
        parent.transform.SetParent(SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform);
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.position = SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.transform.position;
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.rotation = SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.transform.rotation;
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.GetChild(0).transform.SetParent(SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.parent);
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.transform.SetParent(snapPointsParent.transform);

        AttachSnapPoints();
        //Debug.Log("Object SNAPPED");
    }


    private static void AttachSnapPoints()
    {
        // Save Attachable SnapPoints
        SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.currentSnapPointContainer = SnapPointDetectionController.Instance.currentSELECTEDSnapPoint;
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.currentSnapPointContainer = SnapPointDetectionController.Instance.currentDETECTEDSnapPoint;

        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint.DisableSnapPoints();
        SnapPointDetectionController.Instance.currentDETECTEDSnapPoint.DisableSnapPoints();
        Debug.Log("PRODUCT IS ATTACHED");
        EmptycurrentSelectedSnapPoints();
    }

    private static void EmptycurrentSelectedSnapPoints()
    {
        SnapPointDetectionController.Instance.currentSELECTEDSnapPoint = null;
        SnapPointDetectionController.Instance.currentDETECTEDSnapPoint = null;
    }
}
