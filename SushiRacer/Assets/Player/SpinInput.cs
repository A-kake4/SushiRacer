using UnityEngine;
//----------------------------------------
// 回転の入力を管理するクラス
//-----------------------------------------
// 入力方向を8方向に分けて入力ベクトルとして記録し
// 前回フレームと現在フレームが回転方向に変化していれば
// 回転力を高める
// 
// 入力がキーボードの場合
// マウスの移動方向を入力ベクトルとする
// 
// 入力がゲームパッドの場合
// 左スティックの入力方向を入力ベクトルとする
//
// 入力が閾値を超えた場合のみ処理
// 入力値の角度差分が右回転を正、左回転を負とする
//
// 回転速度は絶対値とする

[RequireComponent( typeof( SushiComponent ) )]
public class SpinImput : MonoBehaviour
{
    [SerializeField]
    SushiComponent sushiComponent = null; // 対象のSushiComponent

    [SerializeField, ReadOnly]
    private int nowSpinRotasion = 0;

    public int NowSpinRotasion => nowSpinRotasion;

    [SerializeField, ReadOnly]
    private int nowSpinSpeed = 0;

    public int NowSpinSpeed => nowSpinSpeed;

    [SerializeField, ReadOnly]
    private int spinRate = 1;        // 回転速度の加速値
    [SerializeField, ReadOnly]
    private int decayRate = 1;       // 回転速度の減速値
    [SerializeField, ReadOnly]

    private int maxSpinSpeed = 100;  // 最大回転速度
    public int MaxSpinSpeed => maxSpinSpeed; // 最大回転速度のプロパティ

    private Vector2 oldInput = Vector2.zero;
    private Vector2 nowInput = Vector2.zero;

    [SerializeField, ReadOnly]
    private float inputLimit = 0.1f; // 入力の閾値
    [SerializeField, ReadOnly]
    private float angleLimit = 5f; // 角度差分の閾値

    private void Reset()
    {
        // 初期化処理
        sushiComponent = GetComponent<SushiComponent>();
    }

    private void Start()
    {
        var sushiData = sushiComponent.GetSushiData();
        if ( sushiData != null )
        {
            // スピン速度の設定
            spinRate = sushiData.accelSpinRate;
            decayRate = sushiData.decaySpinRate;
            maxSpinSpeed = sushiData.maxFrontSpeed;
        }
        else
        {
            Debug.LogWarning( "SushiData is not set in SushiComponent." );
        }
    }

    private void Update()
    {
        // 前回の入力を保存
        oldInput = nowInput;

        // ゲームパッドのベクトルを取得
        nowInput = InputManager.Instance.GetActionValue<Vector2>(1, "MainGame", "Spin" );

        // 入力が閾値を超えた場合のみ処理
        if ( nowInput.sqrMagnitude > inputLimit * inputLimit && oldInput.sqrMagnitude > inputLimit * inputLimit )
        {
            // 入力ベクトルを正規化
            Vector2 normalizedNowInput = nowInput.normalized;
            Vector2 normalizedOldInput = oldInput.normalized;

            // 角度を計算
            float oldAngle = Mathf.Atan2(normalizedOldInput.y, normalizedOldInput.x) * Mathf.Rad2Deg;
            float newAngle = Mathf.Atan2(normalizedNowInput.y, normalizedNowInput.x) * Mathf.Rad2Deg;

            // 角度差分（-180～180度に収める）
            float deltaAngle = Mathf.DeltaAngle(oldAngle, newAngle);

            // 角度差分が180に完全に一致する場合は、回転しない
            if ( Mathf.Abs( deltaAngle ) >= 180f )
            {
                deltaAngle = 0f; // 完全に反対方向の入力は無視
            }

            // 回転方向を判定
            if ( Mathf.Abs( deltaAngle ) > angleLimit ) // 最小以上の変化のみ加算
            {
                if ( deltaAngle > 0 )
                {
                    // 左回転
                    nowSpinRotasion -= spinRate;
                }
                else if ( deltaAngle < 0 )
                {
                    // 右回転
                    nowSpinRotasion += spinRate;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // 回転速度を更新
        if ( nowSpinRotasion > 0 )
        {
            nowSpinSpeed += spinRate;
        }
        else if ( nowSpinRotasion < 0 )
        {
            nowSpinSpeed -= spinRate;
        }
        else
        {
            // 回転速度を減速
            if ( nowSpinSpeed > 0 )
            {
                nowSpinSpeed -= decayRate;
            }
            else if ( nowSpinSpeed < 0 )
            {
                nowSpinSpeed += decayRate;
            }
        }
        // 最大回転速度を制限
        nowSpinSpeed = Mathf.Clamp( nowSpinSpeed, -maxSpinSpeed, maxSpinSpeed );
        // 回転方向をリセット
        nowSpinRotasion = 0;
    }

}
