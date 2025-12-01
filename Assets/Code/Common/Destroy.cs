using UnityEngine;

public class Destroy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SelfDestruct(float delay = 0f)
    {
        if (spriteRenderer != null && delay > 0f)
        {
            spriteRenderer.enabled = false;
        }

        Destroy(gameObject, delay);
    }
}
