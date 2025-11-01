using UnityEngine;

public class FloatingButton : MonoBehaviour
{
    public float amplitude = 10f;   // ふわふわの高さ（px）
    public float speed = 2f;        // ふわふわする速さ
    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        rect.anchoredPosition = new Vector2(startPos.x, newY);
    }
}
