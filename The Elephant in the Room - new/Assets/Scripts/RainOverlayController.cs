using UnityEngine;
using UnityEngine.UI;

public class RainOverlayController : MonoBehaviour
{
    public RawImage overlay;
    public Vector2 scrollSpeed = new Vector2(0f, -0.5f);

    void Update()
    {
        if (overlay != null)
        {
            Rect uv = overlay.uvRect;
            uv.position += scrollSpeed * Time.deltaTime;
            overlay.uvRect = uv;
        }
    }
} 