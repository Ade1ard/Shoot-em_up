using DG.Tweening;
using UnityEngine;

public class DOTweentest : MonoBehaviour
{
    void Start()
    {
        transform.DOJump(Vector2.up, 2, 3, 7);
    }

    void Update()
    {
        //transform.DOJump(Vector2.up, 10, 3, 2);
    }
}
