using UnityEngine;

public class SpeedWindEffect : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb = null; // 対象のRigidbody

    [SerializeField, Header("風のエフェクト")]
    private ParticleSystem windEffect = null; // 風のエフェクト

    [SerializeField,Header("最大速度")]
    private float maxSpeed = 100f; // 最大速度の基準値

    [SerializeField, Header("風のエフェクトの量を調整するためのパラメータ")]
    private float maxRateOverTime = 50f; // エフェクト量の最小値と最大値

    private void FixedUpdate()
    {
        // 現在速度を取得
        float speedRate = rb.linearVelocity.sqrMagnitude / ( maxSpeed * maxSpeed );

        //Debug.Log($"Speed Rate: {rb.linearVelocity.magnitude}");

        var emission = windEffect.emission;
        emission.rateOverTime = speedRate * maxRateOverTime;
    }
}
