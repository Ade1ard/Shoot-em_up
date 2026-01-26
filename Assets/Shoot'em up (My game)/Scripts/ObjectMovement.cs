using DG.Tweening;
using UnityEngine;

public enum TrajectoryType
{
    SineWave,
    ZigZag,
    Circle,
    Spiral,
}

public enum MovementType
{
    Linear,
    Curvelinear,
}

public class ObjectMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private MovementType _movementType = MovementType.Linear;
    [SerializeField] private bool _isItEnemy;
    [SerializeField] private bool _upDirection;

    [Header("Linear Movement Settings")]
    [SerializeField] private float _speed = 10;

    [Header("Curvelinear Movement Settings")]
    [SerializeField] private TrajectoryType type = TrajectoryType.SineWave;
    [SerializeField] private float _moveDuration = 30f;
    [SerializeField] private PathType pathType = PathType.CatmullRom;

    [Header("SinWave")]
    public float amplitude = 3f;
    public float frequency = 2f;
    public float verticalDistance = 10f;
    public int resolution = 30;

    [Header("Circle")]
    public float radius = 4f;
    public int loops = 2;
    public bool clockwise = true;
    public int circleResolution = 50;

    [Header("ZigZag")]
    public float zigzagWidth = 3f;
    public int zigzagCount = 5;
    public float zigzagVerticalDistance = 8f;

    [Header("Spiral")]
    public float spiralStartRadius = 4f;
    public float spiralEndRadius = 0.5f;
    public int spiralTurns = 3;

    private void Start()
    {
        switch (_movementType)
        {
            case MovementType.Linear:
                transform.DOMove((_upDirection ? Vector3.up : Vector3.down) * _speed, 2)
                    .SetRelative()
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(gameObject));

                return;

            case MovementType.Curvelinear:
                Vector3[] path = GeneratePath(transform.position);

                var tween =  transform.DOPath(path, _moveDuration, pathType, PathMode.TopDown2D)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(gameObject));

                if (!_isItEnemy)
                    tween.SetLookAt(0.01f);

                return;
        }
    }

    public Vector3[] GeneratePath(Vector3 startPosition)
    {
        switch (type)
        {
            case TrajectoryType.SineWave:
                return GenerateSinePath(startPosition);
            case TrajectoryType.Circle:
                return GenerateCirclePath(startPosition);
            case TrajectoryType.ZigZag:
                return GenerateZigZagPath(startPosition);
            case TrajectoryType.Spiral:
                return GenerateSpiralPath(startPosition);
            default:
                return new Vector3[] {startPosition, startPosition + Vector3.down};
        }
    }

    private Vector3[] GenerateSinePath(Vector3 start)
    {
        Vector3[] path = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);
            float x = Mathf.Sin(t * Mathf.PI * 2 * frequency) * amplitude;
            float y = start.y - t * verticalDistance;

            path[i] = new Vector3(start.x + x, y, start.z);
        }

        return path;
    }

    private Vector3[] GenerateCirclePath(Vector3 start)
    {
        Vector3[] path = new Vector3[circleResolution];
        float direction = clockwise ? 1f : -1f;

        for (int i = 0; i < circleResolution; i++)
        {
            float angle = (float)i / (circleResolution - 1) * Mathf.PI * 2 * loops * direction;
            float x = Mathf.Cos(angle) * radius;
            float y = start.y + Mathf.Sin(angle) * radius * 0.5f;

            path[i] = new Vector3(start.x + x, y, start.z);
        }

        return path;
    }

    private Vector3[] GenerateZigZagPath(Vector3 start)
    {
        int totalPoints = zigzagCount * 10;
        Vector3[] path = new Vector3[totalPoints];

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (float)i / (totalPoints - 1);
            int segment = Mathf.FloorToInt(t * zigzagCount);
            float localT = (t * zigzagCount) - segment;

            float x = Mathf.PingPong(segment, 1) * zigzagWidth;
            if (segment % 2 == 1) x = zigzagWidth - x;

            x = Mathf.SmoothStep(-zigzagWidth, zigzagWidth, localT) * (segment % 2 == 0 ? 1 : -1);
            float y = start.y - t * zigzagVerticalDistance;

            path[i] = new Vector3(start.x + x, y, start.z);
        }

        return path;
    }

    private Vector3[] GenerateSpiralPath(Vector3 start)
    {
        int res = 60;
        Vector3[] path = new Vector3[res];

        for (int i = 0; i < res; i++)
        {
            float t = (float)i / (res - 1);
            float angle = t * Mathf.PI * 2 * spiralTurns;
            float currentRadius = Mathf.Lerp(spiralStartRadius, spiralEndRadius, t);

            float x = Mathf.Cos(angle) * currentRadius;
            float y = start.y - t * 8f;

            path[i] = new Vector3(start.x + x, y, start.z);
        }

        return path;
    }
}