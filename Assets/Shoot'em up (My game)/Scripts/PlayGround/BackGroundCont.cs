using System.Collections.Generic;
using UnityEngine;

public class BackGroundCont : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private List<SpriteRenderer> _backGrounds = new List<SpriteRenderer>();
    [SerializeField] private float _backGroundSpeed;
    [SerializeField] private int _backgroundSpriteCountHorizontal = 1;

    private float _backGroundHeight;
    private float _bottomBoundary;
    private float _stepBackground;

    void Start()
    {
        _bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        _backGroundHeight = _backGrounds[0].bounds.size.y;
        _stepBackground = _backGroundHeight * _backGrounds.Count / _backgroundSpriteCountHorizontal - 0.2f;
        SetStartPosition();
    }

    void Update()
    {
        LoopBackGround();

        MoveBackGround();
    }

    private void LoopBackGround()
    {
        if (_backGrounds.Count == 0) return;

        foreach (var backGround in _backGrounds)
        {
            if (backGround.transform.position.y + _backGroundHeight / 2 < _bottomBoundary)
                backGround.transform.position += new Vector3(0, _stepBackground, 0);
        }
    }

    private void MoveBackGround()
    {
        if (_backGrounds.Count == 0) return;

        foreach (var backGround in _backGrounds)
            backGround.transform.position += Vector3.down * _backGroundSpeed * Time.deltaTime;
    }

    private void SetStartPosition()
    {
        if (_backGrounds.Count > 0)
        {
            int index = 1;
            foreach (var background in _backGrounds)
            {
                background.transform.position += new Vector3(0, (Mathf.Floor(index / _backgroundSpriteCountHorizontal) - 1) * _backGroundHeight, 0);
                index++;
            }
        }
    }
}
