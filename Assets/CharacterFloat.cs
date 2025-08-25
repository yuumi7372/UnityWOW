using UnityEngine;
using System.Collections;

public class CharacterNaturalMove : MonoBehaviour
{
    public float moveUp = 51.7f; // Y方向の移動量
    public float scaleMultiplier = 1.8f; // 拡大率
    public float tiltAngle = 3f; // 左に傾ける角度
    public float duration = 0.15f; // アニメーション時間

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Quaternion originalRotation;
    private Quaternion targetRotation;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + new Vector3(0, moveUp / 100f, 0); // 51.7px ≒ 0.517f（CanvasならAnchoredPositionでpx制御）

        originalScale = transform.localScale;
        targetScale = originalScale * scaleMultiplier;

        originalRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, tiltAngle);

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最終値をしっかり適用
        transform.position = targetPosition;
        transform.localScale = targetScale;
        transform.rotation = targetRotation;
    }
}
