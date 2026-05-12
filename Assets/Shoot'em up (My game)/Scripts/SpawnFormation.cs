using UnityEngine;

public interface ISpawnFormation
{
    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count);
}

[System.Serializable]
public class SpawnRandom: ISpawnFormation
{
    [SerializeField] private float _radius = 3f;

    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count)
    {
        var offset = new Vector3(
                    UnityEngine.Random.Range(-_radius, _radius),
                    UnityEngine.Random.Range(-_radius / 2, _radius / 2),
                    0
                );
        return startPosition + offset;
    }
}

[System.Serializable]
public class SpawnCircle: ISpawnFormation
{
    [SerializeField] private float _radius = 3f;

    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count)
    {
        float angle = (index / (float)count) * Mathf.PI * 2;
        var offset = new Vector3(
            Mathf.Cos(angle) * _radius,
            Mathf.Sin(angle) * _radius,
            0
        );
        return startPosition + offset;
    }
}

[System.Serializable]
public class SpawnLine: ISpawnFormation
{
    [SerializeField] private float _Width = 3f;

    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count)
    {
        float t = index / (float)Mathf.Max(1, count - 1);
        var offset = new Vector3(
            Mathf.Lerp(-_Width, _Width, t),
            0,
            0
        );
        return startPosition + offset;
    }
}

[System.Serializable]
public class SpawnVFormation: ISpawnFormation
{
    public Vector2 _spacing = new Vector2(0.5f, 0.5f);

    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count)
    {
        float centerIndex = (count - 1) / 2f;
        float distanceFromCenter = Mathf.Abs(index - centerIndex);
        var offset = new Vector3(
            (index - centerIndex) * _spacing.x,
            -distanceFromCenter * _spacing.y,
            0
        );
        return startPosition + offset;
    }
}

[System.Serializable]
public class SpawnGrid: ISpawnFormation
{
    public Vector2 _spacing = new Vector2(0.5f, 0.5f);
    public int _columns = 3;

    public Vector3 CalculateSpawnPosition(Vector3 startPosition, int index, int count)
    {
        int row = index / _columns;
        int col = index % _columns;
        var offset = new Vector3(
            (col - (_columns - 1) / 2f) * _spacing.x,
            -row * _spacing.y,
            0
        );
        return startPosition + offset;
    }
}
