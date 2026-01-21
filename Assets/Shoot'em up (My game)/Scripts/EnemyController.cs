using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private int _pathPointsCount = 30;
    [SerializeField] private PathType pathType = PathType.CatmullRom;

    [Header("Wave Settings")]
    [SerializeField] private float _amplitude = 3f;
    [SerializeField] private float _waves = 4f;
    [SerializeField] private float _duration = 30f;
    [SerializeField] private float _verticalDistance = 14f;

    void Start()
    {
        Vector3[] path = GenerateSinePath(transform.position, _amplitude, _waves, _verticalDistance, _pathPointsCount);

        transform.DOPath(path, _duration, pathType, PathMode.Full3D, 10).SetEase(Ease.Linear);
    }

    Vector3[] GenerateSinePath(Vector3 start, float amp, float waves, float yDist, int points)
    {
        Vector3[] path = new Vector3[points];

        for (int i = 0; i < points; i++)
        {
            float t = (float)i / (points - 1);

            float x = Mathf.Sin(t * Mathf.PI * 2 * waves) * amp;

            float y = start.y - t * yDist;

            path[i] = new Vector3(start.x + x, y, start.z);
        }

        return path;
    }
}
