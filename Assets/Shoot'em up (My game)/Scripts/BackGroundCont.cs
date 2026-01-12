using System.Collections.Generic;
using UnityEngine;

public class BackGroundCont : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private List<SpriteRenderer> _backGrounds = new List<SpriteRenderer>();
    [SerializeField] private List<SpriteRenderer> _backGroundClouds = new List<SpriteRenderer>();

    [Header("Settings")]
    [SerializeField] private float _backGroundSpeed;
    [SerializeField] private float _cloudsSpeed;
    [SerializeField] private int _spriteCountHorizontal = 1;

    private float _cloudHeight;
    private float _backGroundHeight;
    private float _bottomBoundary;

    void Start()
    {
        _bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        _cloudHeight = _backGroundClouds[0].bounds.size.y;
        _backGroundHeight = _backGrounds[0].bounds.size.y;

        SetStartPosition();
    }

    void Update()
    {
        MoveBackGround();
        LoopBackGround();
    }

    private void LoopBackGround()
    {
        foreach (var backGround in _backGrounds)
        {
            if (backGround.transform.position.y + _backGroundHeight / 2 < _bottomBoundary)
                backGround.transform.position += new Vector3(0, _backGroundHeight * _backGrounds.Count / _spriteCountHorizontal, 0);
        }

        foreach (var cloud in _backGroundClouds)
        {
            if (cloud.transform.position.y + _cloudHeight / 2 < _bottomBoundary)
                cloud.transform.position += new Vector3(0, _cloudHeight * _backGroundClouds.Count / _spriteCountHorizontal, 0);
        }
    }

    private void MoveBackGround()
    {
        foreach (var backGround in _backGrounds)
            backGround.transform.position += Vector3.down * _backGroundSpeed * Time.deltaTime;

        foreach (var cloud in _backGroundClouds)
            cloud.transform.position += Vector3.down * _cloudsSpeed * Time.deltaTime;
    }

    private void SetStartPosition()
    {
        int index = 1;
        foreach (var cloud in _backGroundClouds)
        {
            index++;
            cloud.transform.position += new Vector3(0, (Mathf.Floor(index / _spriteCountHorizontal) - 1) * _cloudHeight, 0);
        }

        index = 1;
        foreach (var background in _backGrounds)
        {
            index++;
            background.transform.position += new Vector3(0, (Mathf.Floor(index / _spriteCountHorizontal) - 1) * _backGroundHeight, 0);
        }
    }
}
