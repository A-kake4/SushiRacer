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
    [SerializeField] private Rigidbody playerRigidbody; // プレイヤーのRigidbody
    public Rigidbody PlayerRigidbody
    {
        get => playerRigidbody;
        set => playerRigidbody = value;
    }

    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // カメラのオフセット
    [SerializeField] private Vector2 rotationOffset = Vector2.zero; // プレイヤーの方向を向く際の補正角度
    [SerializeField] private float zoomSpeed = 2f; // ズーム速度
    [SerializeField] private float minField = 40f; // 最大ズーム値
    [SerializeField] private float maxField = 100f; // 最大ズーム距離

    [SerializeField] private float maxFieldY = 1f; // 最大Y軸のズーム値

    void LateUpdate()
    {
        if ( playerRigidbody == null )
        {
            Debug.LogError( "Player Transform is not assigned." );
            return;
        }

        var spinImput = playerRigidbody.GetComponent<SpinImput>();
        if ( spinImput == null )
        {
            Debug.LogError( "SpinImput component is not assigned." );
            return;
        }

        // SpinImputから現在の回転速度と最大回転速度を取得
        float maxSpeed = spinImput.MaxSpinSpeed;
        float nowSpeed = spinImput.NowSpinSpeed < 0 ?
                            spinImput.NowSpinSpeed * -1 : spinImput.NowSpinSpeed;

        // 現在の速度に基づいてカメラのField of Viewを計算
        float targetField = Mathf.Lerp( minField, maxField, nowSpeed / maxSpeed );

        var targetOffset = offset;
        // プレイヤーの速度に基づいてY軸のオフセットを調整
        targetOffset.y = Mathf.Lerp( offset.y, offset.y + maxFieldY, nowSpeed / maxSpeed );

        // プレイヤーの正面方向を基準にオフセットを回転
        Vector3 rotatedOffset = playerRigidbody.rotation * targetOffset;


        // プレイヤーの位置に回転されたオフセットを加えた位置を計算
        Vector3 desiredPosition = playerRigidbody.position + rotatedOffset;

        // カメラの位置を更新
        transform.position = desiredPosition;
        // プレイヤーの速度に基づいてカメラのField of Viewを調整
        float speed = playerRigidbody.linearVelocity.magnitude;



        var camera = GetComponent<Camera>();
        if ( camera == null )
        {
            Debug.LogError( "Camera component is not assigned." );
            return;
        }
        // カメラのField of Viewを更新
        camera.fieldOfView = Mathf.Lerp( camera.fieldOfView, targetField, Time.deltaTime * zoomSpeed );

        // プレイヤーの方向を向く
        transform.LookAt( playerRigidbody.position );

        // プレイヤーの方向を向く際に補正角度を適用
        transform.Rotate( rotationOffset.y, rotationOffset.x, 0, Space.Self );
    }

}
