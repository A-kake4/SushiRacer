using UnityEngine;
//---------------------------
// プレイヤーの前方に移動させるクラス
//------------------------------
// 移動量はSpinImputの回転力を利用して
// 最大回転力を1とした場合の、
// プレイヤーの前方への移動量を計算します。

[RequireComponent( typeof( PlayerTotalVelocity3d ) )]
public class PlayerFrontMoveCameraTarget : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private SpinImput spinImput; // SpinImputスクリプトの参照

    [SerializeField, ReadOnly]
    private PlayerTotalVelocity3d playerTotalVelocity; // プレイヤーの総合速度スクリプト
    [SerializeField, ReadOnly]
    private Rigidbody playerRigidbody;

    private void Reset()
    {
        spinImput = GetComponent<SpinImput>();
        playerTotalVelocity = GetComponent<PlayerTotalVelocity3d>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // SpinImputから現在の回転速度を取得
        int spinSpeed = spinImput.NowSpinSpeed;
        int maxSpinSpeed = spinImput.MaxSpinSpeed;

        if ( spinSpeed < 0 )
        {
            spinSpeed *= -1;
        }


        // 最大回転速度を1とした場合の前方移動量を計算
        float moveAmount = (float)spinSpeed / maxSpinSpeed;

        float addSpeed = maxSpinSpeed * moveAmount;

        float forwardSpeed = Vector3.Dot( playerRigidbody.linearVelocity, transform.forward );

        if( forwardSpeed < maxSpinSpeed )
        {
            playerTotalVelocity.AddLinearVelocity( addSpeed * Time.fixedDeltaTime * transform.forward );
        }
    }
}
