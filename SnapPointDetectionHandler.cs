public static class SnapPointDetectionHandler
{
    public static SnapPointTrigger currentSELECTEDSnapPoint;
    public static SnapPointTrigger currentDETECTEDSnapPoint;
    private static float closestDistance;
    public static float ClosestDistance { get => closestDistance; set => closestDistance = value; }
}