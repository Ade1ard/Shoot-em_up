using System.Collections.Generic;
using UnityEngine;

public enum ProjectileOwner
{
    Player,
    Enemy,
    Other
}

public class ProjectilePool
{
    private readonly Dictionary<ProjectileCont, Queue<ProjectileCont>> _inactive = new();
    private readonly HashSet<ProjectileCont> _active = new();
    private readonly Transform _container;

    public ProjectilePool()
    {
        _container = new GameObject("ProjectilePool").transform;
    }

    public ProjectileCont Get(ProjectileCont prefab, Vector3 position, Quaternion rotation, ProjectileOwner owner)
    {
        if (prefab == null)
            return null;

        if (!_inactive.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<ProjectileCont>();
            _inactive[prefab] = queue;
        }

        ProjectileCont projectile = null;
        while (queue.Count > 0 && projectile == null)
            projectile = queue.Dequeue();

        if (projectile == null)
        {
            projectile = Object.Instantiate(prefab, position, rotation);
            projectile.SetPool(this, prefab);
        }

        projectile.transform.SetParent(_container);
        projectile.transform.SetPositionAndRotation(position, rotation);
        projectile.gameObject.SetActive(true);
        projectile.PrepareFromPool(owner, prefab);
        _active.Add(projectile);

        return projectile;
    }

    public void Release(ProjectileCont projectile)
    {
        if (projectile == null || !_active.Remove(projectile))
            return;

        projectile.StopBeforePool();
        projectile.transform.SetParent(_container);
        projectile.gameObject.SetActive(false);

        ProjectileCont prefab = projectile.PrefabKey;
        if (prefab == null)
        {
            Object.Destroy(projectile.gameObject);
            return;
        }

        if (!_inactive.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<ProjectileCont>();
            _inactive[prefab] = queue;
        }

        queue.Enqueue(projectile);
    }

    public void ClearActive(ProjectileOwner owner)
    {
        var projectiles = new List<ProjectileCont>(_active);
        foreach (var projectile in projectiles)
        {
            if (projectile != null && projectile.Owner == owner)
                projectile.ReturnToPool(false);
        }
    }

    public void ClearInactive(ProjectileCont prefab)
    {
        if (prefab == null || !_inactive.TryGetValue(prefab, out var queue))
            return;

        while (queue.Count > 0)
        {
            ProjectileCont projectile = queue.Dequeue();
            if (projectile != null)
                Object.Destroy(projectile.gameObject);
        }
    }

    public void ClearAllActive()
    {
        var projectiles = new List<ProjectileCont>(_active);
        foreach (var projectile in projectiles)
        {
            if (projectile != null)
                projectile.ReturnToPool(false);
        }
    }
}
