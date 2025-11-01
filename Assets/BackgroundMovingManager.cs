using UnityEngine;
using UnityEngine.UI;

public class BackgroundMovingManager : MonoBehaviour
{
    public RawImage image;            // 背景画像
    public float scrollSpeed = 0.05f; // スクロール速度（＋で右、－で左）
    private Vector2 offset = Vector2.zero;

    void Update()
    {
        if (image == null) return;

        // 横方向に動かす
        offset.x += scrollSpeed * Time.deltaTime;

        // スクロールが大きくなりすぎないようにループ
        if (offset.x > 1f) offset.x -= 1f;
        if (offset.x < -1f) offset.x += 1f;

        // 背景を動かす
        image.uvRect = new Rect(offset, image.uvRect.size);
    }
}
