using UnityEngine;
//--------------------------------------
// プレイヤーの総合速度を管理するクラス
//---------------------------------------
// 他スクリプトでプレイヤーの速度を代入する際に
// 競合を避けるために、このスクリプトで合算された速度を
// 参照するようにします。

[RequireComponent( typeof( Rigidbody ) )]
public class PlayerTotalVelocity3d : MonoBehaviour
{
    [SerializeField, Header("プレイヤーRigidbody") ,ReadOnly]
    private Rigidbody playerRigidbody;
    [SerializeField, Header( "設定加速度" ) , ReadOnly]
    private Vector3 totalAngularVelocity = Vector3.zero;
    // 総合実速度
    [SerializeField, Header( "設定速度" ) , ReadOnly]
    private Vector3 totalLinearVelocity = Vector3.zero;

    private bool isInitializedLinear = false; // 総合実速度が設定されたかどうか

    private void Reset()
    {
        // コンポーネントの参照取得
        playerRigidbody = GetComponent<Rigidbody>();
        if ( playerRigidbody != null )
        {
            // xとz軸の回転を固定
            playerRigidbody.constraints
                = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerRigidbody.useGravity = true; // 重力を有効にする
        }
    }

    private void Start()
    {
        PlayerFocusCamera3d playerFocusCamera = FindAnyObjectByType<PlayerFocusCamera3d>();
        playerFocusCamera.PlayerRigidbody = playerRigidbody; // カメラにRigidbodyを設定
    }

    public void AddAngularVelocity(Vector3 vector3 )
    {
        // 総合加速度に加算
        totalAngularVelocity += vector3;
    }

    public void AddLinearVelocity( Vector3 vector3 )
    {
        if ( isInitializedLinear )
        {
            Debug.LogWarning( "PlayerTotalVelocity3d: LinearVelocityは既に総合実速度が設定されています。再設定は無視されます。" );
            return; // 既に総合実速度が設定されている場合は再設定を無視
        }
        // 総合実速度に加算
        totalLinearVelocity += vector3;
    }

    public void SetLinearVelocity( Vector3 vector3 )
    {
        if ( isInitializedLinear )
        {
            Debug.LogWarning( "PlayerTotalVelocity3d: LinearVelocityは既に総合実速度が設定されています。再設定は無視されます。" );
            return; // 既に総合実速度が設定されている場合は再設定を無視
        }

        totalLinearVelocity = totalLinearVelocity + vector3;
        isInitializedLinear = true; // 総合実速度が設定されたことを示す
    }

    private void Update()
    {
        if ( playerRigidbody == null )
        {
            Debug.LogError( "PlayerTotalVelocity3d: Rigidbodyが見つかりません。" );
            return;
        }

        // 総合実速度をRigidbodyの速度に設定
        if ( isInitializedLinear )
        {
            playerRigidbody.linearVelocity = totalLinearVelocity;
        }
        else
        {
            if ( totalLinearVelocity != Vector3.zero )
            {
                totalLinearVelocity.y = playerRigidbody.linearVelocity.y; // RigidbodyのY軸速度を保持

                playerRigidbody.linearVelocity = totalLinearVelocity; // 初期化されていない場合は総合実速度を設定
            }
        }

        // 総合加速度をRigidbodyの角速度に設定
        playerRigidbody.AddForce( totalAngularVelocity, ForceMode.Force );

        // 総合加速度と総合実速度をリセット
        totalAngularVelocity = Vector3.zero;
        totalLinearVelocity = Vector3.zero;
        isInitializedLinear = false; // 総合実速度の初期化フラグをリセット
    }

}
