using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundsCont : MonoBehaviour
{
    [SerializeField] private List<BoxCollider2D> _screenBoundsColliders = new List<BoxCollider2D>();

    void Start()
    {
        float bottomBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;
        float leftBoundary = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;

        int indexHor = 0;
        int indexVert = 0;

        foreach (BoxCollider2D Collider in _screenBoundsColliders)
        {
            if (Collider.size.x > Collider.size.y)
            {
                if (indexHor == 0)
                {
                    Collider.offset = new Vector2(0, bottomBoundary);
                    Collider.size = new Vector2(leftBoundary * -2, 0.5f);
                    indexHor = 1;
                }
                else
                {
                    Collider.offset = new Vector2(0, -bottomBoundary);
                    Collider.size = new Vector2(leftBoundary * -2, 0.5f);
                }
            }
            else
            {
                if (indexVert == 0)
                {
                    Collider.offset = new Vector2(leftBoundary, 0);
                    Collider.size = new Vector2(0.5f, bottomBoundary * -2);
                    indexVert = 1;
                }
                else
                {
                    Collider.offset = new Vector2(-leftBoundary, 0);
                    Collider.size = new Vector2(0.5f, bottomBoundary * -2);
                }
            }
        }
    }
}