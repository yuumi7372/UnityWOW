using UnityEngine;
using UnityEngine.UI;

public class BackgroundMovingManager : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("❌ RawImage がアタッチされてないよ！");
        }
    }

    void Update()
    {
        if (rawImage == null) return;

        // UVを縦にスクロール
        Vector2 offset = rawImage.uvRect.position;
        offset.y += scrollSpeed * Time.deltaTime;

        rawImage.uvRect = new Rect(offset, rawImage.uvRect.size);
    }
}
