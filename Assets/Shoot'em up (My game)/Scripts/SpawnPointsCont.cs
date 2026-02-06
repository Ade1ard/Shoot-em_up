using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsCont : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private float _boundsOffset = 1;

    void Start()
    {
        float bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        float leftBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        foreach (Transform point in spawnPoints)
        {
            if (point.position.x <= -leftBoundary && point.position.x >= leftBoundary)
            {
                if (point.position.x > 0)
                    point.position = new Vector3(-leftBoundary / 2, -bottomBoundary + _boundsOffset, 0);
                else
                    point.position = new Vector3(leftBoundary / 2, -bottomBoundary + _boundsOffset, 0);
            }

            if (point.position.x <= leftBoundary)
                point.position = new Vector3(leftBoundary - _boundsOffset, (-bottomBoundary - bottomBoundary) / 4, 0);

            if (point.position.x >= -leftBoundary)
                point.position = new Vector3(-leftBoundary + _boundsOffset, (-bottomBoundary - bottomBoundary) / 4, 0);
        }
    }
}
