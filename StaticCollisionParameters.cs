public class StaticCollisionParameters
{
    private static bool collisionDetected;
    public static bool CollisionDetected { get => collisionDetected; set => collisionDetected = value; }

    private static float timerLimit = 0.3f;
    public static float TimerLimit { get => timerLimit; }
}
