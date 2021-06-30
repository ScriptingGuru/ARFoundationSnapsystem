using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPointDetectionController : MonoBehaviour
{
    private static SnapPointDetectionController instance;
    public static SnapPointDetectionController Instance { get => instance; }
    private static bool triggerZoneDetected;
    public bool TriggerZoneDetected { get => triggerZoneDetected; set => triggerZoneDetected = value; }

    public SnapPointTrigger currentSELECTEDSnapPoint;
    public SnapPointTrigger currentDETECTEDSnapPoint;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
}
