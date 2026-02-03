using DG.Tweening;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    void Start()
    {
        GetComponentInChildren<Animator>().SetFloat("StartOffset", Random.Range(0f, 1f));
    }
}