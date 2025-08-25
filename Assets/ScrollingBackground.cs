using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public float backgroundWidth = 19.2f;

    private Transform backgroundA;
    private Transform backgroundB;

    void Start()
    {
        backgroundA = transform.GetChild(0);
        backgroundB = transform.GetChild(1);
    }

    void Update()
    {
        float move = scrollSpeed * Time.deltaTime;

        backgroundA.position += Vector3.left * move;
        backgroundB.position += Vector3.left * move;

        // 画面外に出た背景を右にループ
        if (backgroundA.position.x <= -backgroundWidth)
        {
            backgroundA.position += Vector3.right * backgroundWidth * 2;
            SwapBackgrounds();
        }

        if (backgroundB.position.x <= -backgroundWidth)
        {
            backgroundB.position += Vector3.right * backgroundWidth * 2;
            SwapBackgrounds();
        }
    }

    // AとBを入れ替えて処理をスッキリにょ
    void SwapBackgrounds()
    {
        var temp = backgroundA;
        backgroundA = backgroundB;
        backgroundB = temp;
    }
}
