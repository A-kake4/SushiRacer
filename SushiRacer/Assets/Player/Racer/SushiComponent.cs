using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BaseComponent<SushiItem, SushiDataScriptableObject> ), true )]
public class SushiComponentEditor : BaseComponentEditor{}
#endif

public enum SushiMode
{
    Event = 0, // イベントモード
    Normal = 1, // 通常モード
    DriftWall = 2 // ドリフトウォールモード
}

public class SushiComponent : BaseComponent<SushiItem, SushiDataScriptableObject>
{
    [SerializeField,Header("プレイヤーの番号")]
    private int playerNumber = 0; // プレイヤーの番号

    public int PlayerNumber
    {
        set => playerNumber = value;
        get => playerNumber;
    }

    [SerializeField]
    private SushiMode sushiMode = SushiMode.Event;

    [SerializeField, Header( "プレイヤーを追従するカメラ" )]
    private PlayerFocusCamera3d playerFocusCamera; // プレイヤーを追従するカメラ

    [SerializeField, ReadOnly]
    private Rigidbody rb;
    public Rigidbody Rigidbody
    {
        get => rb;
    }

    [SerializeField, ReadOnly]
    private SpinImput spinImput; // SpinImputスクリプトの参照

    [SerializeField, Header( "ドリフトウォールの移動コンポーネント" )]
    private SplineAnimateRigidbody splineAnimateRigidbody; // スプラインアニメーションの参照
    public SplineAnimateRigidbody SplineAnimateRigidbody
    {
        get => splineAnimateRigidbody;
    }

    [SerializeField, ReadOnly]
    private PlayerTotalVelocity3d playerTotalVelocity; // プレイヤーの総合速度スクリプト



    [SerializeField, ReadOnly]
    private float accelSideRate = 10f; // プレイヤーの加速度
    [SerializeField, ReadOnly]
    private float maxSideSpeed = 20f; // プレイヤーの最大速度

    [SerializeField, ReadOnly]
    private float rotationMinSpeed = 0.1f;
    [SerializeField, ReadOnly]
    private float rotationMaxSpeed = 3f; // プレイヤーの回転速度
    [SerializeField, ReadOnly]
    private float gierRatio = 1f; // プレイヤーのギア比

    private Vector3 moveDirection; // プレイヤーの移動方向

    private bool started = false;

    public SushiItem GetSushiData()
    {
        return dataSource.GetItem(selectedItemNumber);
    }

    private void Start()
    {
        var sushiData = dataSource.GetItem( selectedItemNumber );
        accelSideRate = sushiData.accelSideRate;
        maxSideSpeed = sushiData.maxSideSpeed;
        rotationMaxSpeed = sushiData.rotationMaxSpeed;
        rotationMinSpeed = sushiData.rotationMinSpeed;
        gierRatio = sushiData.gierRatio;
    }

    private void Reset()
    {
        // コンポーネントの参照取得
        playerTotalVelocity = GetComponent<PlayerTotalVelocity3d>();
        spinImput = GetComponent<SpinImput>();
        rb = GetComponent<Rigidbody>();
    }

    public SushiMode GetSushiMode()
    {
        return sushiMode;
    }

    public void SetSushiMode( SushiMode mode )
    {
        sushiMode = mode;
        switch (mode)
        {
            case SushiMode.Event: // イベントモード

                // イベントモードでは回転を固定
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true; // 動的にしない

                playerTotalVelocity.SetLinearVelocity( Vector3.zero );

                playerFocusCamera.FocusMode = 0; // カメラのフォーカスモードをイベントに設定

                break;
            case SushiMode.Normal: // 通常モード

                // 通常モードでは回転を固定
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                                | RigidbodyConstraints.FreezeRotationY
                                | RigidbodyConstraints.FreezeRotationZ;

                rb.useGravity = true; // 重力を有効にする

                rb.isKinematic = false; // 動的にする
                playerFocusCamera.FocusMode = 1; // カメラのフォーカスモードを通常に設定
                break;
            case SushiMode.DriftWall: // ドリフトウォールモード

                // ドリフトウォールモードでは回転を固定
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                                | RigidbodyConstraints.FreezeRotationY
                                | RigidbodyConstraints.FreezeRotationZ;

                rb.useGravity = false; // 重力を無効にする

                rb.isKinematic = false; // 動的にする

                playerTotalVelocity.SetLinearVelocity( Vector3.zero ); // 総合実速度をリセット
                playerFocusCamera.FocusMode = 2; // カメラのフォーカスモードをドリフトウォールに設定

                break;
            default:
                Debug.LogWarning( "Invalid sushi mode selected." );
                break;
        }
    }

    private void FixedUpdate()
    {
        if ( sushiMode == SushiMode.Event )
        {
            if ( CountDownAnimation_Tsuji.instance.GetIsStarted() )
            {
                if ( !started )
                {
                    SetSushiMode( SushiMode.Normal );

                    started = true;
                }
            }

            return;
        }
        if ( sushiMode == SushiMode.Normal )
        {
            PlayerMove();
            return;
        }
        if (sushiMode == SushiMode.DriftWall)
        {
            splineAnimateRigidbody.SpeedFactor = spinImput.NowSpinSpeed * 0.02f * gierRatio; // スピン速度に応じて移動速度を調整

            var moveInput = spinImput.MoveInput;
            if (moveInput.sqrMagnitude < 0.01f)
            {
                // 入力がほとんどない場合は移動しない
            }
            else
            {
                float nowSpinSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;
                int maxSpinSpeed = spinImput.MaxSpinSpeed;

                float rotationSpeed = Mathf.Lerp(
                    rotationMaxSpeed * Time.fixedDeltaTime,
                    rotationMinSpeed * Time.fixedDeltaTime,
                    nowSpinSpeed / maxSpinSpeed );

                splineAnimateRigidbody.OffsetPsitionY += rotationSpeed * moveInput.x * 5f; // 入力に応じてカメラのY軸回転を調整
            }
            return;
        }
    }

    private void PlayerMove()
    {
        FrontMoveCameraTarget();
        SideMoveCameraTarget();
    }

    private void FrontMoveCameraTarget()
    {
        // SpinImputから現在の回転速度を取得
        int spinSpeed = spinImput.NowSpinSpeed;
        int maxSpinSpeed = spinImput.MaxSpinSpeed;

        if (spinSpeed < 0)
        {
            spinSpeed *= -1;
        }

        // 最大回転速度を1とした場合の前方移動量を計算
        float moveAmount = (float)spinSpeed / maxSpinSpeed;

        float addSpeed = maxSpinSpeed * moveAmount;

        float forwardSpeed = Vector3.Dot( rb.linearVelocity, transform.forward );

        if (forwardSpeed < maxSpinSpeed)
        {
            playerTotalVelocity.AddLinearVelocity( addSpeed * Time.fixedDeltaTime * transform.forward );
        }
    }

    private void SideMoveCameraTarget()
    {
        // プレイヤーの右方向を取得（カメラの方向からプレイヤーの方向へ変更）
        Vector3 playerRight = transform.right;

        playerRight.y = 0; // y成分を除去して水平面での右方向を取得
        playerRight.Normalize(); // 正規化
        if (playerRight.sqrMagnitude < 0.01f)
        {
            Debug.LogWarning( "Player right direction is too small, cannot move." );
            return; // プレイヤーの右方向が小さすぎる場合は処理を中止
        }

        // 入力を取得（移動のみ）
        float horizontalInput =spinImput.MoveInput.x;

        // 移動方向を計算
        moveDirection = playerRight * horizontalInput;

        // プレイヤーの左右速度を更新
        Vector3 targetVelocity = moveDirection * maxSideSpeed;
        // 現在の速度を取得
        Vector3 currentVelocity = rb.linearVelocity;
        currentVelocity.y = 0; // Y成分を除去して水平面での速度を取得

        // 目標速度に向けて加速または減速
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 accelerationVector = moveDirection.normalized * accelSideRate;

            //playerTotalVelocity.AddAngularVelocity(
            //    Vector3.MoveTowards( currentVelocity, targetVelocity, accelerationVector.magnitude ) );

            // 回転の最大速度を1として最大回転を行う
            float nowSpinSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;
            int maxSpinSpeed = spinImput.MaxSpinSpeed;

            float rotationSpeed = Mathf.Lerp(
                rotationMaxSpeed * Time.fixedDeltaTime,
                rotationMinSpeed * Time.fixedDeltaTime,
                nowSpinSpeed / maxSpinSpeed );

            // プレイヤーの向きを移動方向に合わせて回転
            Quaternion targetRotation = Quaternion.LookRotation( moveDirection, Vector3.up );
            rb.rotation = Quaternion.Slerp( rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime );
        }
    }
}