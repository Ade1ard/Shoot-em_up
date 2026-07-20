using UnityEngine;

public class VFXReturnToPool : MonoBehaviour
{
    private ParticleSystem _prefabKey;
    public ParticleSystem PrefabKey => _prefabKey;
    
    private VFXPool _pool;
    private ParticleSystem _particleSystem;

    public void SetPool(VFXPool pool, ParticleSystem prefabKey)
    {
        _pool = pool;
        _prefabKey = prefabKey;
        _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystem != null)
        {
            var mainModule = _particleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            mainModule.playOnAwake = false;
        }
    }

    void OnParticleSystemStopped()
    {
        if (_pool != null && _prefabKey != null)
            _pool.Release(_particleSystem);
    }
    
    void OnDestroy()
    {
        _pool = null;
        _particleSystem = null;
    }
}
