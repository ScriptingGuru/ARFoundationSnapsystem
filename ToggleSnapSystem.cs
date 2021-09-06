using UnityEngine;

[RequireComponent(typeof(SnapSystem))]
public class ToggleSnapSystem : MonoBehaviour
{
    private SnapSystem snapTriggerSystem;

    private void Awake()
    {
        snapTriggerSystem = this.GetComponent<SnapSystem>();
        snapTriggerSystem.enabled = false;
    }

    private void OnEnable()
    {
        EventBus.Instance.OnSelectGO += EnableSnappingSystem;
        EventBus.Instance.OnDeselectGO += DisableSnappingSystem;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSelectGO -= EnableSnappingSystem;
        EventBus.Instance.OnDeselectGO -= DisableSnappingSystem;
    }

    private void DisableSnappingSystem(GameObject gameObject)
    {
        ToggleSnapTriggerSystem(gameObject, false);
    }

    private void EnableSnappingSystem(GameObject gameObject, ProductPrefabDataManager productPrefabDataManager) 
    {
        ToggleSnapTriggerSystem(gameObject, true);
    }

    private void ToggleSnapTriggerSystem(GameObject selectedGo, bool value)
    {
        if (selectedGo == this.gameObject)
        {
            snapTriggerSystem.enabled = value;
        }
    }
}
