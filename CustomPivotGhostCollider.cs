using UnityEngine;

public class CustomPivotGhostCollider : MonoBehaviour
{
    // Only use this class If there are two SnapPoints on an
    //snap object that is not symmertical.
    [Header("Add existing snappoint")]
    public Transform snapPointA;
    public Transform snapPointB;
}