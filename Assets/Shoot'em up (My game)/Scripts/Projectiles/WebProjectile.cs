using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    private List<ObjectMovement> _objectMovements = new List<ObjectMovement>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ObjectMovement>(out var objMovement))
        {
            StartCoroutine(RandomStop(objMovement));
            _objectMovements.Add(objMovement);
        }
    }

    private IEnumerator RandomStop(ObjectMovement objMovement)
    {
        yield return new WaitForSeconds(Random.Range(0, 0.15f));
        if (objMovement != null)
            objMovement.StopMove();
    }

    private void OnDisable()
    {
        foreach (var objMovement in _objectMovements)
            if (objMovement != null)
                objMovement.ContinueMove();
        _objectMovements.Clear();
    }
}
