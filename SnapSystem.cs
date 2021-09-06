using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will snap two snappable objects together by using snapPoints Triggers <see cref="SnapPointTrigger"/>
/// The current selected gameobject and the detected SnapPoint gameobject
/// This occurs when two Snapoint triggers detect each other and a finger first touches, drags, and then releases the screen.
/// </summary>


public class SnapSystem : MonoBehaviour
{
    [Header("SnapPoint Parent", order = 0)]
    [SerializeField] private GameObject snapPointsParent;
    
    [Header("Ghost Highlighter References", order = 1)]
    [SerializeField] private GameObject ghostHighlighterParent;
    [SerializeField] private GameObject ghostModel;
    [SerializeField] private GhostCollisionVerfication ghostCollider;
    
    [Header("Collision Detecter Parent", order = 2)]
    [SerializeField] private CollisionDetecter collisionDetecter;
    
    private Transform ParentObject;
    
    bool isDraggingGO;
    private bool delay;
    private float ghostTimeDelay = 0.4f;

    private Vector3 startPosition;
    private Vector3 startGhostCollisionPosition;
    private float minDistMoveF = 0.1f;
    private float minDistAfterGhostCollisionMoveF = 0.2f;

    private void Awake()
    {
        if (ghostHighlighterParent)
        {
            ghostModel.SetActive(false);
        }
        snapPointsParent.SetActive(false);
        collisionDetecter.gameObject.SetActive(false);
        ParentObject = GetComponent<Transform>();
        EventBus.Instance.OnDragStart += OnDragStart;
        EventBus.Instance.OnDragEnd += OnDragEnd;
        EventBus.Instance.OnSelectGO += OnSelectGameObject;
        EventBus.Instance.OnDeselectGO += Instance_OnDeselectGO;
        EventBus.Instance.OnDrag += OnDrag;
        EventBus.Instance.OnGameObjectDrag += Instance_OnGameObjectDrag;
    }

    private void Instance_OnDeselectGO(GameObject go)
    {
        DisableGhostPrefab();
    }

    private void OnSelectGameObject(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        ghostCollider.GetComponent<Collider>().enabled = false;
        if (SelectionHandler.SelectedGO == gameObject)
        {
            startPosition = transform.position;
            snapPointsParent.SetActive(false);
        }
        else
        {
            snapPointsParent.SetActive(true);
        }
    }

    private void Start()
    {
        ghostHighlighterParent.transform.parent = this.transform.parent;
    }

    private void OnDrag(Vector2 touchPos)
    {
        if (StaticCollisionParameters.CollisionDetected)
            return;

        float distance = Vector3.Distance(startPosition, transform.position);
        float ghostDistance = Vector3.Distance(startGhostCollisionPosition, transform.position);
        if (!isDraggingGO && distance > minDistMoveF)
        {
            isDraggingGO = true;
            collisionDetecter.gameObject.SetActive(true);
            snapPointsParent.SetActive(true);
            startPosition = transform.position;
        }
       
        if (ghostCollider.CollisionDetectedByGhostprefab && ghostDistance > minDistAfterGhostCollisionMoveF)
        {
            if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
            {
               ghostCollider.CollisionDetectedByGhostprefab = false;
            }
        }

        if (!delay && SnapPointDetectionHandler.currentSELECTEDSnapPoint != null && SnapPointDetectionHandler.currentDETECTEDSnapPoint != null)
        {
            MoveGhostPrefab();
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

    public void InitalizeSnapProcess()
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {
            
            ToggleGhostHighlighter();
        }
        // Check Ghost Collision
        // Toggle Ghost Collider and Highlighter Collider

    }
    public void ResetSnapProcess()
    {
        // Reset 
        ghostCollider.GetComponent<Collider>().enabled = false;
        DisableGhostPrefab();
    }

    public void ToggleGhostHighlighter()
    {
        if (StaticCollisionParameters.CollisionDetected)
        {
            ResetSnapProcess();
            return;
        }

        ghostCollider.GetComponent<Collider>().enabled = true;
       
        if (!delay)
        GhostSnapper();
    }
    private void CheckGhostCollision()
    {
        if (ghostCollider.CollisionDetectedByGhostprefab)
        {
            startGhostCollisionPosition = transform.position;
            // Ghost Colllision Detected
            ResetSnapProcess();
        }
        else
        {
            // Ghost Colllision NOT Detected
            HighLightSnapCollider();
        }
    }

    private void HighLightSnapCollider()
    {
        ghostModel.SetActive(true);
        delay = true;
        StartCoroutine(SetGhostDelay());
        ghostCollider.CollisionDetectedByGhostprefab = false;
    }


    private void DisableGhostPrefab()
    {
        if (!delay)
        {
            ghostModel.SetActive(false);
        }
    }

    IEnumerator SetGhostDelay()
    {
        float timer = 0f;
        while (timer < ghostTimeDelay)
        {
            timer += Time.deltaTime;
            if (StaticCollisionParameters.CollisionDetected)
            {
                timer = ghostTimeDelay;
            }
            yield return 0;
        }
        delay = false;
        if (!SnapPointDetectionHandler.currentSELECTEDSnapPoint || !SnapPointDetectionHandler.currentDETECTEDSnapPoint)
        {
            ghostModel.SetActive(false);
        }
        if (SnapPointDetectionHandler.currentSELECTEDSnapPoint && SnapPointDetectionHandler.currentDETECTEDSnapPoint)
        {
            GhostSnapper();
        }
    }

    private void Instance_OnGameObjectDrag(Vector3 worldPosition)
    {
        if (!SnapPointDetectionHandler.currentSELECTEDSnapPoint || !SnapPointDetectionHandler.currentDETECTEDSnapPoint)
        {
            DisableGhostPrefab();
        }

        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        { 
            collisionDetecter.gameObject.SetActive(true);
            snapPointsParent.SetActive(true);
        }
    }

    public void OnDragStart(Vector2 vector2)
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {
            ghostCollider.CollisionDetectedByGhostprefab = false;
        }
    }

    public void OnDragEnd(Vector2 vector2)
    {
        if (SelectionHandler.SelectedGO != null && SelectionHandler.SelectedGO == this.gameObject)
        {
            if (isDraggingGO)
            {
                isDraggingGO = false;
                if (!SnapPointDetectionHandler.currentSELECTEDSnapPoint || !SnapPointDetectionHandler.currentDETECTEDSnapPoint)
                {
                    snapPointsParent.SetActive(false);
                    ghostCollider.GetComponent<Collider>().enabled = false;
                    ghostCollider.CollisionDetectedByGhostprefab = false;
                }

                else if (SnapPointDetectionHandler.currentSELECTEDSnapPoint && SnapPointDetectionHandler.currentDETECTEDSnapPoint)
                {
                    if (!ghostCollider.CollisionDetectedByGhostprefab && !StaticCollisionParameters.CollisionDetected)
                    {
                        snapPointsParent.SetActive(true);
                        Snap(ParentObject);
                        ResetEventsBeforeSnap();
                    }
                }
            }
        }
    }

    private void ResetEventsBeforeSnap()
    {
        SelectionHandler.latestSelectedGO = SelectionHandler.SelectedGO;
        ghostModel.SetActive(false);
        GameObject tempGo = SelectionHandler.SelectedGO;
        SelectionHandler.SelectedGO = null;
        EventBus.Instance.DeselectGO(tempGo);
        snapPointsParent.SetActive(false);
    }

    private void GhostSnapper()
    {
        MoveGhostPrefab(); // Move the GhostHighLighter to correct Detected SnapPoint Position
        CheckGhostCollision(); // Verify if a GhostPrefab Collision has occurred.
    }


    private void MoveGhostPrefab()
    {
       
        if (SnapPointDetectionHandler.currentDETECTEDSnapPoint != null)
        {
            ghostHighlighterParent.transform.position = SnapPointDetectionHandler.currentDETECTEDSnapPoint.transform.position;
            ghostHighlighterParent.transform.rotation = SnapPointDetectionHandler.currentDETECTEDSnapPoint.transform.rotation;
            ghostHighlighterParent.transform.localRotation *= Quaternion.Euler(0, -180, 0);

            // This code will invert the ghost collider transform to the correct position if SnapPoint has two snapPoints that are not symmetrical.
            if (ghostHighlighterParent.GetComponent<CustomPivotGhostCollider>())
                InvertScaleTransform();
        }
    }

    private void InvertScaleTransform()
    {
            if (ghostHighlighterParent.GetComponent<CustomPivotGhostCollider>().snapPointA.transform == SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform)
            {
                ghostHighlighterParent.transform.localScale = new Vector3(-1, ghostHighlighterParent.transform.localScale.y, ghostHighlighterParent.transform.localScale.z);
            }
            else if (ghostHighlighterParent.GetComponent<CustomPivotGhostCollider>().snapPointB.transform == SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform)
            {
                ghostHighlighterParent.transform.localScale = new Vector3(1, ghostHighlighterParent.transform.localScale.y, ghostHighlighterParent.transform.localScale.z);
            }
    }

    private void Snap(Transform parent) // Move the product to correct SnapPoint Position
    {
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.parent = this.transform.parent;
        parent.transform.SetParent(SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform);
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.position = SnapPointDetectionHandler.currentDETECTEDSnapPoint.transform.position;
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.rotation = SnapPointDetectionHandler.currentDETECTEDSnapPoint.transform.rotation;
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.GetChild(0).transform.SetParent(SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.parent);
        SnapPointDetectionHandler.currentSELECTEDSnapPoint.transform.SetParent(snapPointsParent.transform);

        EventBus.Instance.Snap();
        EmptycurrentSelectedSnapPoints();
    }

    private void EmptycurrentSelectedSnapPoints()
    {
        SnapPointDetectionHandler.currentSELECTEDSnapPoint = null;
        SnapPointDetectionHandler.currentDETECTEDSnapPoint = null;
    }
}
