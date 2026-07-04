using DG.Tweening;
using UnityEngine;
using System.Collections;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private bool _isItEnemy;

    [Header("Movement Settings")]
    [SerializeReference, SubclassSelector]
    private IMovementType _movementType;

    [Header("Direction Settings")]
    [SerializeReference, SubclassSelector]
    private IDirectionGenerator _dirGenerator;

    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = transform.position;
    }

    public void Init(IDirectionGenerator dirGenerator = null, IMovementType movementType = null)
    {
        if (movementType != null)
            _movementType = movementType;
        if (dirGenerator != null)
            _dirGenerator = dirGenerator;
    }

    public void StartMove(Vector3 startPosition = default, Vector3 spawnerPos = default)
    {
        if (_movementType != null)
            _movementType.Move(transform, startPosition, spawnerPos, _dirGenerator, _isItEnemy);
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
                ProjectileCont projectile = GetComponent<ProjectileCont>();
                if (projectile != null)
                    projectile.ReturnToPool(false);
                else
                {
                    DOTween.Kill(transform);
                    Destroy(gameObject);
                }
            }
        }
    }

    public void StopMove()
    {
        _movementType?.Stop();
        DOTween.Kill(transform);
    }

    private void OnDestroy()
    {
        StopMove();
    }
}

public interface IMovementType
{
    public void Move(UnityEngine.Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy);
    public void Stop();
}

[System.Serializable]
public class LinearMove: IMovementType
{
    [Header("Linear Movement Settings")]
    [SerializeField] private float _linearSpeed = 10;

    public void Move(UnityEngine.Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy)
    {
        Vector3 direction = dirGenerator.GenerateDirection(startPosition, spawnerPos);
        transform.DOMove(direction * _linearSpeed, 3)
            .SetRelative()
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        if (!isItEnemy)
            transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
    }

    public void Stop()
    {

    }
}

[System.Serializable]
public class CurveLinearMove: IMovementType
{
    [Header("Curvelinear Movement Settings")]
    [SerializeReference, SubclassSelector]
    private IPathGenerator _path;

    [SerializeField] private float _moveDuration = 30f;
    [SerializeField] private float _moveDurationOffset = 5f;
    [SerializeField] private PathType pathType = PathType.CatmullRom;

    private Tween _tween;

    public void Move(UnityEngine.Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy)
    {
        Vector3[] path = _path.GeneratePath(transform.position, dirGenerator.GenerateDirection(startPosition, spawnerPos));
        var tween = transform.DOPath(path, _moveDuration + Random.Range(-_moveDurationOffset, _moveDurationOffset), pathType, PathMode.TopDown2D)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        if (!isItEnemy)
            tween.SetLookAt(0.01f);

        _tween = tween;
    }

    public void Stop()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
            _tween = null;
        }
    }
}

[System.Serializable]
public class BetweenPointMove: IMovementType
{
    [Header("BetweenPoints Movement Settings")]
    [SerializeField] private float _moveDuration = 6f;
    [SerializeField] private float _inPointTime = 3f;
    [SerializeField] private float _inPointTimeOffset = 1f;

    private Sequence _sequence;
    private bool _isMoving;

    public void Move(UnityEngine.Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy)
    {
        Stop();
        _isMoving = true;
        StartNextMovement(transform, isItEnemy);
    }

    private void StartNextMovement(Transform transform, bool isItEnemy)
    {
        if (!_isMoving) return;

        Vector3 point = new Vector3(
            Random.Range(G._leftBoundary + 1, -G._leftBoundary - 1),
            Random.Range(G._bottomBoundary / 2, -G._bottomBoundary - 1),
            0
        );

        float waitTime = _inPointTime + Random.Range(-_inPointTimeOffset, _inPointTimeOffset);

        _sequence = DOTween.Sequence();

        _sequence.AppendInterval(waitTime);

        var moveTween = transform.DOMove(point, _moveDuration)
            .SetEase(Ease.InOutSine);

        if (!isItEnemy)
        {
            moveTween.OnStart(() => {
                Vector3 dir = (point - transform.position).normalized;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            });
        }

        _sequence.Append(moveTween);

        _sequence.OnComplete(() => StartNextMovement(transform, isItEnemy));

        _sequence.Play();
    }

    public void Stop()
    {
        _isMoving = false;
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
            _sequence = null;
        }
    }
}

[System.Serializable]
public class HomingMove : IMovementType
{
    [Header("Homing Settings")]
    [SerializeField] private float _startSpeed = 5f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private float _searchRadius = 20f;
    [SerializeField] private float _nearIgnoreRadius = 0;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private TargetType _targetType;
    [SerializeField] private bool _spriteRotate;

    private enum TargetType
    {
        Enemy,
        Player
    }

    private Transform _transform;
    private Rigidbody2D _rb;
    private Transform _target;
    private Transform _spriteTransform;
    private Quaternion _spriteStartLocalRotation;
    private float _currentSpeed;
    private bool _isActive;

    public void Move(Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy)
    {
        _transform = transform;
        _rb = transform.GetComponent<Rigidbody2D>();
        _isActive = true;
        _currentSpeed = _startSpeed;
        _spriteTransform = transform.GetComponentInChildren<SpriteRenderer>()?.transform;
        _spriteStartLocalRotation = transform.GetComponent<ProjectileCont>().StartSpriteRotation;

        _target = FindClosestTarget();

        transform.GetComponent<MonoBehaviour>().StartCoroutine(HomingRoutine());
    }

    private IEnumerator HomingRoutine()
    {
        var wait = new WaitForFixedUpdate();

        while (_isActive && _transform != null)
        {
            _currentSpeed = Mathf.Min(_currentSpeed + _acceleration * Time.fixedDeltaTime, _maxSpeed);

            if (_target == null || !_target.gameObject.activeInHierarchy || Vector3.Distance(_transform.position, _target.transform.position) < 0.1f)
                _target = FindClosestTarget();

            Vector3 moveDirection = _target != null
                ? (_target.position - _transform.position).normalized
                : _rb.linearVelocity.normalized;

            Vector3 currentDir = _rb.linearVelocity.magnitude > 0.01f
                ? _rb.linearVelocity.normalized
                : moveDirection;

            Vector3 newDir = Vector3.RotateTowards(
                currentDir,
                moveDirection,
                _rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0
            ).normalized;

            _rb.linearVelocity = newDir * _currentSpeed;

            if (_spriteTransform != null && _spriteRotate)
            {
                float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
                _spriteTransform.localRotation = Quaternion.Euler(0, 0, angle) * _spriteStartLocalRotation;
            }

            yield return wait;
        }
    }

    private Transform FindClosestTarget()
    {
        var hits = Physics2D.OverlapCircleAll(_transform.position, _searchRadius, _targetLayer);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            switch (_targetType)
            {
                case TargetType.Enemy:
                    if (!hit.TryGetComponent<Enemy>(out _)) continue;
                    break;

                case TargetType.Player:
                    if (!hit.TryGetComponent<Player>(out _)) continue;
                    break;
            }

            float dist = Vector3.Distance(_transform.position, hit.transform.position);
            if (dist < minDist && dist > _nearIgnoreRadius)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }

        return closest;
    }

    public void Stop()
    {
        _isActive = false;
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }
}

[System.Serializable]
public class CircleFollowMove: IMovementType
{
    [SerializeField] private float _orbitRadius = 3f;
    [SerializeField] private float _orbitSpeed = 3f;
    [SerializeField] private bool _clockWise = false;
    [SerializeField] private bool _spriteRotate;
    
    private Transform _transform;
    private Transform _target;
    private Rigidbody2D _rb;
    private Transform _spriteTransform;
    private Quaternion _spriteStartLocalRotation;
    private bool _isActive;
    private float _currentAngle;
    
    public void Move(Transform transform, Vector3 startPosition, Vector3 spawnerPos, IDirectionGenerator dirGenerator, bool isItEnemy)
    {
        _transform = transform;
        _rb =  transform.GetComponent<Rigidbody2D>();

        _target = GetTarget(spawnerPos);

        _spriteTransform = transform.GetComponentInChildren<SpriteRenderer>()?.transform;
        _spriteStartLocalRotation = transform.GetComponent<ProjectileCont>().StartSpriteRotation;
        
        _isActive = true;
        transform.GetComponent<MonoBehaviour>().StartCoroutine(FollowingRoutine());
    }

    private IEnumerator FollowingRoutine()
    {
        while (_isActive)
        {
            _currentAngle += _orbitSpeed * Time.deltaTime * (_clockWise? -1:1) * Mathf.Deg2Rad;
            Vector2 center = _target.position;
            
            Vector2 targetPos = center + new Vector2(
                Mathf.Cos(_currentAngle) * _orbitRadius,
                Mathf.Sin(_currentAngle) *  _orbitRadius
                );
            
            Vector2 moveDirection = targetPos - (Vector2)_transform.position;
            if (_spriteTransform != null && _spriteRotate && moveDirection.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                _spriteTransform.localRotation = Quaternion.Euler(0, 0, angle) * _spriteStartLocalRotation;
            }

            if (_rb != null)
                _rb.MovePosition(targetPos);
            
            yield return null;
        }
    }

    private Transform GetTarget(Vector3 spawnerPos)
    {
        var objs = Object.FindObjectsByType<Health>();
        float minDist = float.MaxValue;
        var closestObj = objs[0];
        foreach (var obj in objs)
        {
            var dist = Vector3.Distance(obj.transform.position, spawnerPos);
            if (dist < minDist)
            {
                minDist = dist;
                closestObj = obj;
            }
        }

        return  closestObj.transform;
    }

    public void Stop()
    {
        _isActive = false;
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }
}

public interface IPathGenerator
{
    public Vector3[] GeneratePath(Vector3 start, Vector3 direction);
}

[System.Serializable]
public class GenerateSpiralPath: IPathGenerator
{
    [Header("Spiral")]
    public float spiralStartRadius = 4f;
    public float spiralEndRadius = 0.5f;
    public int spiralTurns = 3;
    public float spiralDistance = 10f;

    public Vector3[] GeneratePath(Vector3 start, Vector3 direction)
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
}

[System.Serializable]
public class GenerateCirclePath: IPathGenerator
{
    [Header("Circle")]
    public float radius = 4f;
    public bool clockwise = true;
    public int circleResolution = 50;

    public Vector3[] GeneratePath(Vector3 start, Vector3 dir)
    {
        Vector3[] path = new Vector3[circleResolution];
        float direction = clockwise ? 1f : -1f;

        for (int i = 0; i < circleResolution; i++)
        {
            float angle = (float)i / (circleResolution - 1) * Mathf.PI * 2 * direction;
            float x = Mathf.Cos(angle) * radius;
            float y = start.y + Mathf.Sin(angle) * radius;

            path[i] = new Vector3(start.x + x - radius, y, start.z);
        }

        return path;
    }
}

[System.Serializable]
public class GenerateSinePath: IPathGenerator
{
    [Header("SinWave")]
    public float amplitude = 3f;
    public float frequency = 2f;
    public float sinDistance = 10f;
    public int resolution = 30;

    public Vector3[] GeneratePath(Vector3 start, Vector3 direction)
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
}

public interface IDirectionGenerator
{
    public Vector3 GenerateDirection(Vector3 startPosition, Vector3 spawnerPosition);
}

[System.Serializable]
public class SimpleDir: IDirectionGenerator
{
    [SerializeField] private bool _upDirection;
    [SerializeField] private float _directionOffset = 0.25f;

    public Vector3 GenerateDirection(Vector3 startPosition = default, Vector3 spawnerPos = default)
    {
        return _upDirection ? new Vector3(Random.Range(-_directionOffset, _directionOffset), 1, 0) : new Vector3(Random.Range(-_directionOffset, _directionOffset), -1, 0);
    }
}

[System.Serializable]
public class ToPlayerDir: IDirectionGenerator
{
    public Vector3 GenerateDirection(Vector3 startPosition, Vector3 spawnerPos = default)
    {
        return (G._player.transform.position - startPosition).normalized;
    }
}

[System.Serializable]
public class AdaptiveDir: IDirectionGenerator
{
    [SerializeField] private float _directionOffset = 0.25f;

    public Vector3 GenerateDirection(Vector3 startPosition, Vector3 spawnerPos = default)
    {
        if (startPosition.x < G._leftBoundary + 5)
            return new Vector3(1, Random.Range(-_directionOffset, 0), 0);

        if (startPosition.x > -G._leftBoundary - 5)
            return new Vector3(-1, Random.Range(-_directionOffset, 0), 0);

        return new Vector3(Random.Range(-_directionOffset, _directionOffset), -1, 0);
    }
}

[System.Serializable]
public class FromSpawnerDir: IDirectionGenerator
{
    public Vector3 GenerateDirection(Vector3 startPosition, Vector3 spawnerPos)
    {
        return (startPosition - spawnerPos).normalized;
    }
}

[System.Serializable]
public class FromSpawnerDirNormalized : IDirectionGenerator
{
    private static readonly Vector3[] Directions = {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
        new Vector3(1, 1, 0).normalized,
        new Vector3(-1, 1, 0).normalized,
        new Vector3(1, -1, 0).normalized,
        new Vector3(-1, -1, 0).normalized,
    };

    public Vector3 GenerateDirection(Vector3 startPosition, Vector3 spawnerPos)
    {
        Vector3 raw = (startPosition - spawnerPos).normalized;

        Vector3 closest = Directions[0];
        float maxDot = float.MinValue;

        foreach (var dir in Directions)
        {
            float dot = Vector3.Dot(raw, dir);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = dir;
            }
        }

        return closest;
    }
}
