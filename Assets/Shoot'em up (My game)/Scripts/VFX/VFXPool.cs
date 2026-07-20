using System.Collections.Generic;
using UnityEngine;

public class VFXPool
{
    private readonly Dictionary<ParticleSystem, Queue<ParticleSystem>> _inactive = new();
    private readonly HashSet<ParticleSystem> _active = new();
    private readonly Transform _container;

    public VFXPool()
    {
        _container = new GameObject("VFXPool").transform;
    }

    public ParticleSystem Get(ParticleSystem prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
            return null;

        if (!_inactive.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<ParticleSystem>();
            _inactive[prefab] = queue;
        }

        ParticleSystem vfx = null;
        while (queue.Count > 0 && vfx == null)
            vfx = queue.Dequeue();

        if (vfx == null)
        {
            vfx = Object.Instantiate(prefab, position, rotation, parent != null? parent : _container);
            
            var vfxReturnToPool = vfx.gameObject.AddComponent<VFXReturnToPool>();
            vfxReturnToPool.SetPool(this, prefab);
        }

        vfx.transform.SetParent(parent != null? parent : _container);
        vfx.transform.SetPositionAndRotation(position, rotation);
        vfx.gameObject.SetActive(true);
        
        _active.Add(vfx);

        return vfx;
    }

    public void Release(ParticleSystem vfx)
    {
        if (vfx == null || !_active.Remove(vfx))
            return;

        vfx.transform.SetParent(_container);
        vfx.gameObject.SetActive(false);

        ParticleSystem prefab = vfx.gameObject.GetComponent<VFXReturnToPool>().PrefabKey;
        if (prefab == null)
        {
            Object.Destroy(vfx.gameObject);
            return;
        }

        if (!_inactive.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<ParticleSystem>();
            _inactive[prefab] = queue;
        }

        queue.Enqueue(vfx);
    }
}