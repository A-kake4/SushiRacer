using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class SplineAnimateRigidbody : MonoBehaviour
{
    // スプラインコンテナへの参照（インスペクターで設定）
    public SplineContainer splineContainer;
    // スプライン上を移動する速さ
    public float speed = 1f;
    // パスに合わせて回転させるかどうか
    public bool orientToPath = true;
    // オブジェクトの進行方向から見たオフセット
    public Vector3 offset;

    // スプライン上の進行パラメータ（0～1と想定）
    private float distance;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (splineContainer == null || splineContainer.Spline == null)
        {
            Debug.LogError("SplineContainer or Spline is not set.");
            enabled = false; // スクリプトを無効化
            return;
        }
        // 初期位置をスプラインの開始位置に設定
        distance = 0f;
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition(distance);
        Quaternion rot = orientToPath 
            ? Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(distance), Vector3.up) 
            : Quaternion.identity;
        Vector3 initialPosition = splinePos + rot * offset;
        rb.MovePosition(initialPosition);
    }

    private void FixedUpdate()
    {
        if (splineContainer == null || splineContainer.Spline == null)
            return;

        // 時間に沿って進行パラメータを更新 (ループする場合の例)
        distance += speed * Time.fixedDeltaTime;
        if (distance > 1f)
            distance -= 1f;

        // スプライン上の目標位置を取得し、オフセットを適用
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition(distance);
        Quaternion currentRot = orientToPath 
            ? Quaternion.LookRotation(splineContainer.Spline.EvaluateTangent(distance), Vector3.up) 
            : Quaternion.identity;
        Vector3 targetPosition = splinePos + currentRot * offset;
        rb.MovePosition(targetPosition);

        if (orientToPath)
        {
            // スプライン上の接線方向を利用して向きを調整
            Vector3 forward = splineContainer.Spline.EvaluateTangent(distance);
            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
                rb.MoveRotation(targetRotation);
            }
        }
    }
}
}