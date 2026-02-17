using UnityEngine;

public class Player : Health
{
    [Header("Canvas")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private float _canvasFollowSpeed = 35;

    private Vector3 v = Vector3.zero;

    protected override void Death()
    {
        
    }

    private void Update()
    {
        _playerCanvas.transform.position = Vector3.SmoothDamp(_playerCanvas.transform.position, transform.position, ref v, 1 / _canvasFollowSpeed);
    }
}
