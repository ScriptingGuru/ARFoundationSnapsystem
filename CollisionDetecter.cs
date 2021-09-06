using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script detect other CollisionDetecter colliders and toggles
/// the static bool collisionDetected <see cref="StaticCollisionParameters"/>.
/// </summary>

public class CollisionDetecter : MonoBehaviour
{
    [SerializeField] ProductID productId;
    [SerializeField] MeshRenderer highLighterCollision;

    private GameObject lastDetectedGo;

    public bool MoveToSafeAreaBool { get; private set; }

    private Vector3 collisionStartPosition;
    private Vector3 normalizedDirection;
    private Vector3 collisionEntryPointPosition;
    private Vector3 otherColliderPivotPoint;
    
    private float timer;
    private bool initalEntryPointCollision;
    private float collisionDistanceOffset = 1f;

    private void Awake()
    {
        if (highLighterCollision.enabled)
            highLighterCollision.enabled = false;
    }

    private void OnEnable()
    {
        EventBus.Instance.OnDragEnd += Instance_OnDragEndOnCollision;
        EventBus.Instance.OnDeselectGO += Instance_OnDeselectGO;
        EventBus.Instance.OnSelectGO += Instance_OnSelectGO;
    }

    private void Instance_OnSelectGO(GameObject go, ProductPrefabDataManager productPrefabDataManager)
    {
        highLighterCollision.enabled = false;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnDragEnd -= Instance_OnDragEndOnCollision;
        EventBus.Instance.OnDeselectGO -= Instance_OnDeselectGO;
        EventBus.Instance.OnSelectGO -= Instance_OnSelectGO;
    }

    private void Instance_OnDeselectGO(GameObject go)
    {
        if (go == productId.gameObject)
        {
            highLighterCollision.enabled = false;
        }
    }
    private void Instance_OnDragEndOnCollision(Vector2 _touchPos)
    {
        if (SelectionHandler.SelectedGO == productId.gameObject)
        {
            if (StaticCollisionParameters.CollisionDetected)
            {
                // Snap back to collision entry point position
                //productId.transform.position = collisionStartPosition;

                //// To prevent collision detection, set a minor position offset on selected gameobject.
                //float xAxis = normalizedDirection.x;
                //float zAxis = normalizedDirection.z;
                //Vector3 direction = new Vector3(xAxis, 0, zAxis);
                //productId.transform.position += direction * collisionDistanceOffset;
                //initalEntryPointCollision = false;
                // Make an extra check to ensure the gameobject isn't colliding with an other gameobject.
                //StartCoroutine(MoveToSafeAreaCoroutine(direction));
            }
        }
    }
    private IEnumerator MoveToSafeAreaCoroutine(Vector3 dir)
    {
        // yield return new WaitForSeconds(0.2f);

        while (StaticCollisionParameters.CollisionDetected)
        {
            productId.transform.position += dir * collisionDistanceOffset;
            yield return null;
        }
        highLighterCollision.enabled = false;
        initalEntryPointCollision = false;
        yield return null;
    }
    private void Update()
    {
        if (productId.gameObject != SelectionHandler.SelectedGO)
            return;

        // Debug.LogError(collisionDetected);
        timer += Time.deltaTime;
        if (timer > StaticCollisionParameters.TimerLimit)
        {
            if (StaticCollisionParameters.CollisionDetected)
            {
                ToggleCollisions(false);
            }
        }
    }

    private void OnTriggerStay(Collider _other)
    {
        if (productId.gameObject != SelectionHandler.SelectedGO)
            return;

        if (productId.gameObject != _other.gameObject && _other.GetComponent<CollisionDetecter>())
        {
            if (!initalEntryPointCollision && !StaticCollisionParameters.CollisionDetected)
            {
                initalEntryPointCollision = true;
                collisionStartPosition = productId.transform.position;

                // (To Get direction, acquire the Collision Entry Point
                // and Center Position on the detected object.)
                otherColliderPivotPoint = _other.transform.position;
                collisionEntryPointPosition = _other.ClosestPoint(productId.transform.position);
                normalizedDirection = (collisionEntryPointPosition - otherColliderPivotPoint).normalized;
            }
            timer = 0;
            Debug.Log("Collision Detected" + productId.transform);
            lastDetectedGo = _other.gameObject;
            ToggleCollisions(true);

        }
    }

    private void ToggleCollisions(bool detected)
    {
        StaticCollisionParameters.CollisionDetected = detected;
        highLighterCollision.enabled = detected;
    }
}
