using UnityEngine;
//--------------------------------------
// 3dゲームのプレイヤーを追従するカメラ
//--------------------------------------
// このスクリプトは、プレイヤーの動きに合わせてカメラを追従させるためのものです。
// プレイヤーの正面方向の後ろに、カメラの位置を更新し、プレイヤーの方を向きます。
// プレイヤーの方向を向く際に補正をかけることも出来る
// プレイヤーの速度に比例してFieldを ズームイン・アウトします。

public class PlayerFocusCamera3d : MonoBehaviour
{
    [SerializeField, Header( "カメラのモード" ), Tooltip( "0: 自由, 1: オブジェクトの正面, 2: Y軸の転換可能" )]
    private int focusMode = 0; // モードの初期値（必要に応じて設定）
    public int FocusMode
    {
        get => focusMode;
        set => focusMode = value;
    }

    [SerializeField] private Rigidbody playerRigidbody; // プレイヤーのRigidbody
    public Rigidbody PlayerRigidbody
    {
        get => playerRigidbody;
        set => playerRigidbody = value;
    }

    private Vector3 offsetPosition = new Vector3( 0, 5, -10 ); // カメラのオフセット
    private Camera cameraObject; // カメラコンポーネント
    [SerializeField] private Vector2 rotationOffset = Vector2.zero; // プレイヤーの方向を向く際の補正角度

    [SerializeField] private float zoomSpeed = 2f; // ズーム速度
    [SerializeField] private float minField = 40f; // 最大ズーム値
    [SerializeField] private float maxField = 100f; // 最大ズーム距離

    [SerializeField] private float maxFieldY = 1f; // 最大Y軸のズーム値

    private void Start()
    {
        offsetPosition = transform.localPosition;
        cameraObject = GetComponent<Camera>();
    }


    void LateUpdate()
    {
        if (focusMode == 0)
        {
            // 自由モードの場合は何もしない
            return;
        }

        if (playerRigidbody == null)
        {
            Debug.LogWarning( "Player Transform is not assigned." );
            return;
        }

        var spinImput = playerRigidbody.GetComponent<SpinImput>();
        if (spinImput == null)
        {
            Debug.LogError( "SpinImput component is not assigned." );
            return;
        }

        // SpinImputから現在の回転速度と最大回転速度を取得
        float maxSpeed = spinImput.MaxSpinSpeed;
        float nowSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;

        // 現在の速度に基づいてカメラのField of Viewを計算
        float targetField = Mathf.Lerp( minField, maxField, nowSpeed / maxSpeed );

        var targetOffset = offsetPosition;

        // プレイヤーの速度に基づいてY軸のオフセットを調整
        targetOffset.y = Mathf.Lerp( targetOffset.y, targetOffset.y + maxFieldY, nowSpeed / maxSpeed );

        // プレイヤーの正面方向を基準にオフセットを回転
        Vector3 rotatedOffset = playerRigidbody.rotation * targetOffset;

        // プレイヤーの位置に回転されたオフセットを加えた位置を計算（カメラ位置）
        Vector3 desiredPosition = playerRigidbody.position + rotatedOffset;
        transform.position = desiredPosition;

        // カメラのField of Viewを更新
        if (cameraObject == null)
        {
            Debug.LogError( "Camera component is not assigned." );
            return;
        }
        cameraObject.fieldOfView = Mathf.Lerp( cameraObject.fieldOfView, targetField, Time.deltaTime * zoomSpeed );

        // モード毎にカメラの注視ターゲットを変更
        Vector3 lookTarget = playerRigidbody.position + playerRigidbody.transform.forward;

        // オブジェクトの正面を即時に向く
        transform.LookAt( lookTarget );
        transform.Rotate( rotationOffset.y, rotationOffset.x, 0, Space.Self );
    }

}