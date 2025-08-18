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
    [SerializeField, ReadOnly]
    private float brakeSpeed = 0.5f; // ブレーキの減速率

    private Vector2 oldMoveInput = Vector2.zero;
    private Vector2 nowMoveInput = Vector2.zero;

    // ブレーキ入力のフラグ
    private bool oldBrakeInput = false; // 前回フレームのブレーキ入力
    private bool brakeInput = false; // ブレーキ入力のフラグ
    private int brakeInputFrameCount = 0; // ブレーキ入力のフレームカウント

    private int oldSpinSpeed = 0; // ブレーキ前の回転速度を保存するための変数

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
            brakeSpeed = sushiData.brakeSpeed;
        }
        else
        {
            Debug.LogWarning( "SushiData is not set in SushiComponent." );
        }
    }

    private void Update()
    {
        // 前回の入力を保存
        oldMoveInput = nowMoveInput;

        // ゲームパッドのベクトルを取得
        nowMoveInput = InputManager.Instance.GetActionValue<Vector2>(1, "MainGame", "Spin" );

        // 入力が閾値を超えた場合のみ処理
        if (nowMoveInput.sqrMagnitude > inputLimit * inputLimit && oldMoveInput.sqrMagnitude > inputLimit * inputLimit )
        {
            // 入力ベクトルを正規化
            Vector2 normalizedNowInput = nowMoveInput.normalized;
            Vector2 normalizedOldInput = oldMoveInput.normalized;

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
        // ブレーキ入力を取得
        oldBrakeInput = brakeInput; // 前回フレームのブレーキ入力を保存
        brakeInput = InputManager.Instance.GetActionValue<bool>( 1, "MainGame", "Brake" );

        // ブレーキ入力がある場合は回転速度を減速
        if (brakeInput)
        {
            if (!oldBrakeInput)
            {
                // ブレーキ入力が新たに開始された場合はフレームカウントをリセット
                brakeInputFrameCount = 0;
                oldSpinSpeed = nowSpinSpeed; // ブレーキ前の回転速度を保存
            }

            brakeInputFrameCount++;
            // ブレーキ入力が連続している場合は減速
            if (brakeInputFrameCount > 8)
            {
                nowSpinSpeed = (int)( nowSpinSpeed * brakeSpeed );
            }
        }
        else if (oldBrakeInput)
        {
            if (brakeInputFrameCount > 60)
            {
                // ブレーキ入力が解除された場合は回転速度を元に戻す
                nowSpinSpeed = (int)( oldSpinSpeed * 0.9f ); // ブレーキ前の回転速度を復元
            }

            // ドリフト中の場合は解除
            if (sushiComponent.GetSushiMode() == SushiMode.DriftWall)
            {
                sushiComponent.SplineAnimateRigidbody.StopMovement();
                sushiComponent.SetSushiMode( SushiMode.Normal ); // ドリフトモードを解除
            }

            nowSpinSpeed = -nowSpinSpeed;
        }

        bool attackInput = InputManager.Instance.GetActionValue<bool>( 1, "MainGame", "Attack" );
        
        // 攻撃入力がある場合は回転速度を逆回転
        if (attackInput)
        {
            nowSpinSpeed = -nowSpinSpeed;
        }

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
