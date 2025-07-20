using UnityEngine;

/// <summary>
/// カメラの向きを基準としたプレイヤーの移動方向を取得するスクリプト
/// </summary>
[RequireComponent( typeof( PlayerTotalVelocity3d ) )]
public class PlayerSideMoveCameraTarget : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private PlayerTotalVelocity3d playerTotalVelocity;

    [SerializeField, ReadOnly]
    private Rigidbody playerRigidbody; // プレイヤーのRigidbody

    [SerializeField, ReadOnly]
    private SpinImput spinImput;

    [SerializeField]
    private float acceleration = 10f; // プレイヤーの加速度
    [SerializeField]
    private float maxSpeed = 20f; // プレイヤーの最大速度

    [SerializeField]
    private float rotationMinSpeed = 0.1f;
    [SerializeField]
    private float rotationMaxSpeed = 3f; // プレイヤーの回転速度

    private Vector3 moveDirection; // プレイヤーの移動方向

    private void Reset()
    {
        // コンポーネントの参照取得
        playerTotalVelocity = GetComponent<PlayerTotalVelocity3d>();
        playerRigidbody = GetComponent<Rigidbody>();
        spinImput = GetComponent<SpinImput>();
    }

    private void FixedUpdate()
    {
        // プレイヤーの右方向を取得（カメラの方向からプレイヤーの方向へ変更）
        Vector3 playerRight = transform.right;

        playerRight.y = 0; // y成分を除去して水平面での右方向を取得
        playerRight.Normalize(); // 正規化
        if ( playerRight.sqrMagnitude < 0.01f )
        {
            Debug.LogWarning( "Player right direction is too small, cannot move." );
            return; // プレイヤーの右方向が小さすぎる場合は処理を中止
        }

        // 入力を取得（移動のみ）
        float horizontalInput = InputManager.Instance.GetActionValue<Vector2>("MainGame", "Move").x;

        // 移動方向を計算
        moveDirection = playerRight * horizontalInput;

        // プレイヤーの左右速度を更新
        Vector3 targetVelocity = moveDirection * maxSpeed;
        // 現在の速度を取得
        Vector3 currentVelocity = playerRigidbody.linearVelocity;
        currentVelocity.y = 0; // Y成分を除去して水平面での速度を取得

        // 目標速度に向けて加速または減速
        if ( moveDirection.sqrMagnitude > 0.01f )
        {
            Vector3 accelerationVector = moveDirection.normalized * acceleration;

            playerTotalVelocity.AddAngularVelocity(
                Vector3.MoveTowards( currentVelocity, targetVelocity, accelerationVector.magnitude ) );

            // 回転の最大速度を1として最大回転を行う
            float nowSpinSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;
            float maxSpinSpeed = spinImput.MaxSpinSpeed;
            float rotationSpeed = Mathf.Lerp( 
                rotationMaxSpeed * Time.fixedDeltaTime,
                rotationMinSpeed * Time.fixedDeltaTime,
                nowSpinSpeed / maxSpinSpeed );

            // プレイヤーの向きを移動方向に合わせて回転
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            playerRigidbody.rotation = Quaternion.Slerp( playerRigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime );
        }

    }
}
