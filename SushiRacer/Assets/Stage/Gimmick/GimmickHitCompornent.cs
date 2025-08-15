using UnityEngine;
using System;

public class GimmickHitCompornent : MonoBehaviour
{
    [SerializeField, Header( "ヒットするギミックのレイヤー" )]
    protected LayerMask gimmickLayerMask = default;

    [SerializeField, Header( "ギミックヒット時の挙動" )]
    protected BaseGimmickMoveCompornent gimmickMove = default;

    private void Reset()
    {
        // ギミックのヒットエフェクトの初期化
        gimmickMove = GetComponent<BaseGimmickMoveCompornent>();
    }

    private void OnTriggerEnter( Collider other )
    {
        ProcessCollision( other.gameObject, gimmick => gimmick.GimmickPlayTriggerEnter( gimmickMove ) );
    }

    private void OnTriggerStay( Collider other )
    {
        ProcessCollision( other.gameObject, gimmick => gimmick.GimmickPlayTriggerStay( gimmickMove ) );
    }

    private void OnTriggerExit( Collider other )
    {
        ProcessCollision( other.gameObject, gimmick => gimmick.GimmickPlayTriggerExit( gimmickMove ) );
    }

    private void OnCollisionEnter( Collision collision )
    {
        ProcessCollision( collision.gameObject, gimmick => gimmick.GimmickPlayCollisionEnter( gimmickMove ) );
    }

    private void OnCollisionStay( Collision collision )
    {
        ProcessCollision( collision.gameObject, gimmick => gimmick.GimmickPlayCollisionStay( gimmickMove ) );
    }

    private void OnCollisionExit( Collision collision )
    {
        ProcessCollision( collision.gameObject, gimmick => gimmick.GimmickPlayCollisionExit( gimmickMove ) );
    }

    private void ProcessCollision( GameObject target, Action<BaseGimmickPlayCompornent> action )
    {
        if (!IsLayerValid( target.layer ))
            return;

        if (target.TryGetComponent<BaseGimmickPlayCompornent>( out var gimmick ))
        {
            action( gimmick );
        }
    }

    private bool IsLayerValid( int layer )
    {
        // レイヤーマスクが指定されたレイヤーを含むかどうかをチェック
        return ( gimmickLayerMask & ( 1 << layer ) ) != 0;
    }
}