using DG.Tweening;
using UnityEngine;

public enum TrajectoryType
{
    SineWave,
    Circle,
    Spiral,
}

public enum MovementType
{
    Linear,
    Curvelinear,
}

public enum DirectionType
{
    Simple,
    Adaptive,
    ToPlayer,
}

public class ObjectMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private MovementType _movementType = MovementType.Linear;
    [SerializeField] private bool _isItEnemy;
    [SerializeField] private bool _upDirection;

    [Header("DirectionSettings")]
    [SerializeField] private DirectionType _directionType = DirectionType.Simple;
    [SerializeField] private float _directionOffset = 0.25f;

    [Header("Linear Movement Settings")]
    [SerializeField] private float _speed = 10;

    [Header("Curvelinear Movement Settings")]
    [SerializeField] private TrajectoryType _type = TrajectoryType.SineWave;
    [SerializeField] private float _moveDuration = 30f;
    [SerializeField] private float _moveDurationOffset = 5f;
    [SerializeField] private PathType pathType = PathType.CatmullRom;

    [Header("SinWave")]
    public float amplitude = 3f;
    public float frequency = 2f;
    public float sinDistance = 10f;
    public int resolution = 30;

    [Header("Circle")]
    public float radius = 4f;
    public int loops = 2;
    public bool clockwise = true;
    public int circleResolution = 50;

    [Header("Spiral")]
    public float spiralStartRadius = 4f;
    public float spiralEndRadius = 0.5f;
    public int spiralTurns = 3;
    public float spiralDistance = 10f;

    private Vector3 _startPosition;

    public void StartMove(Vector3 spawnPosition = default)
    {
        switch (_movementType)
        {
            case MovementType.Linear:
                Vector3 direction = CalculateDirection(spawnPosition);
                transform.DOMove(direction * _speed, 3)
                    .SetRelative()
                    .SetEase(Ease.Linear);

                if (!_isItEnemy && _directionType != DirectionType.Simple)
                    transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

                return;

            case MovementType.Curvelinear:
                Vector3[] path = GeneratePath(transform.position, CalculateDirection(spawnPosition));
                var tween = transform.DOPath(path, _moveDuration + Random.Range(-_moveDurationOffset, _moveDurationOffset), pathType, PathMode.TopDown2D)
                    .SetEase(Ease.Linear);

                if (!_isItEnemy)
                    tween.SetLookAt(0.01f);

                return;
        }
    }

    public Vector3 CalculateDirection(Vector3 spawnPosition = default)
    {
        if (spawnPosition == default)
            return _upDirection? Vector3.up : Vector3.down;

        switch (_directionType)
        {
            case DirectionType.Simple:
                return _upDirection ? Vector3.up : Vector3.down;
            case DirectionType.ToPlayer:
                return (Object.FindAnyObjectByType<PlayerMovement>().transform.position - spawnPosition).normalized;
            case DirectionType.Adaptive:
                return CalculateAdaptiveDirection(spawnPosition).normalized;
            default:
                return _upDirection ? Vector3.up : Vector3.down;
        }
    }

    Vector3 CalculateAdaptiveDirection(Vector3 spawnPosition)
    {
        float leftBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        if (spawnPosition.x < leftBoundary + 2)
            return new Vector3(1, Random.Range(-_directionOffset, 0), 0);

        if (spawnPosition.x > -leftBoundary - 2)
            return new Vector3(-1, Random.Range(-_directionOffset, 0), 0);

        return new Vector3(Random.Range(-_directionOffset, _directionOffset), -1, 0);
    }

    public Vector3[] GeneratePath(Vector3 startPosition, Vector3 direction)
    {
        switch (_type)
        {
            case TrajectoryType.SineWave:
                return GenerateSinePath(startPosition, direction);
            case TrajectoryType.Circle:
                return GenerateCirclePath(startPosition);
            case TrajectoryType.Spiral:
                return GenerateSpiralPath(startPosition, direction);
            default:
                return new Vector3[] {startPosition, startPosition + Vector3.down};
        }
    }

    private Vector3[] GenerateSinePath(Vector3 start, Vector3 direction)
    {
        Vector3[] path = new Vector3[resolution];

        Vector3 dirNormalized = direction.normalized;
        Vector3 perpendicular = Vector3.Cross(dirNormalized, Vector3.forward).normalized;

        for (int i = 0; i < resolution; i++)
        {
            float t = (float)i / (resolution - 1);

            Vector3 basePosition = start + dirNormalized * (sinDistance * t);

            float sineValue = Mathf.Sin(t * Mathf.PI * 2 * frequency) * amplitude;
            Vector3 offset = perpendicular * sineValue;

            path[i] = basePosition + offset;
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

    private Vector3[] GenerateSpiralPath(Vector3 start, Vector3 direction)
    {
        int res = 60;
        Vector3[] path = new Vector3[res];

        Vector3 dirNormalized = direction.normalized;
        Vector3 perpendicular = Vector3.Cross(dirNormalized, Vector3.forward).normalized;

        for (int i = 0; i < res; i++)
        {
            float t = (float)i / (res - 1);
            float angle = t * Mathf.PI * 2 * spiralTurns;
            float currentRadius = Mathf.Lerp(spiralStartRadius, spiralEndRadius, t);

            Vector3 basePosition = start + dirNormalized * (spiralDistance * t);

            Vector3 spiralOffset = (dirNormalized * Mathf.Cos(angle) + perpendicular * Mathf.Sin(angle)) * currentRadius;

            path[i] = basePosition + spiralOffset;
        }

        return path;
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TriggerScreenBounds"))
        {
            if (_isItEnemy)
            {
                DOTween.Kill(transform);

                transform.position = _startPosition;
                StartMove(_startPosition);
            }
            else
            {
                DOTween.Kill(transform);
                Destroy(gameObject);
            }
        }
    }
}