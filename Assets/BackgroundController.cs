using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public int sortingOrder = -14; // Ensure background is behind other objects

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}
