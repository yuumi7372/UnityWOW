using UnityEngine;

public class PlanetMover : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // 上下に揺れる幅
    public float floatFrequency = 1f;   // 揺れる速さ
    public Vector2 moveRangeX = new Vector2(-5f, 5f); // 左右に動く範囲
    public Vector2 moveRangeY = new Vector2(-3f, 3f); // 上下に動く範囲
    public float speedX = 0.5f;         // 横移動の速度
    public float speedY = 0.2f;         // 縦移動の速度

    private Vector3 startPos;
    private float offset;

    void Start()
    {
        startPos = transform.position;
        offset = Random.Range(0f, 2f * Mathf.PI); // 個別の動きにする
    }

    void Update()
    {
        // Sin波でぷかぷか
        float yOffset = Mathf.Sin(Time.time * floatFrequency + offset) * floatAmplitude;
        float xOffset = Mathf.Sin(Time.time * floatFrequency * 0.5f + offset) * floatAmplitude * 0.5f;

        Vector3 pos = startPos + new Vector3(xOffset, yOffset, 0);

        // ランダム方向にゆっくり移動
        pos.x += Mathf.PingPong(Time.time * speedX + offset, moveRangeX.y - moveRangeX.x) + moveRangeX.x;
        pos.y += Mathf.PingPong(Time.time * speedY + offset, moveRangeY.y - moveRangeY.x) + moveRangeY.x;

        transform.position = pos;
    }
}
