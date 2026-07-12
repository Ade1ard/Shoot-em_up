using UnityEngine;

public class ProjectileCasterInitializer : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _prjDamage = 10;
    [SerializeField] private float _shootDelay = 2;
    [SerializeField] private int _projectileCount = 1;
    
    private ProjectileCaster _projectileCaster;

    private void Start()
    {
        _projectileCaster = GetComponent<ProjectileCaster>();
        
        StartShooting();
    }

    private void OnEnable()
    {
        StartShooting();
    }

    private void StartShooting()
    {
        if (_projectileCaster != null)
        {
            _projectileCaster.TakeStats(_prjDamage, _shootDelay,  _projectileCount);
            _projectileCaster.IsShooting(true);
        }
    }
}
