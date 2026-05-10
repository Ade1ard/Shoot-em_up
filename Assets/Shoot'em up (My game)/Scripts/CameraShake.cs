using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Default parameters")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _strength = 0.2f;

    private Tween _tween;

    public void Shake(float duration, float strength)
    {
        if (_tween != null)
            _tween.Kill(true);

        _tween = Camera.main.transform.DOShakePosition(duration, strength);
    }

    public void Shake()
    {
        if (_tween != null)
            _tween.Kill(true);

        _tween = Camera.main.transform.DOShakePosition(_duration, _strength);
    }
}
