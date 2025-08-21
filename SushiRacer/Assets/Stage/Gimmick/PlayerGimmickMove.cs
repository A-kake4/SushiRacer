using UnityEngine;
using UnityEngine.Splines;

public class PlayerGimmickMove : BaseGimmickMoveCompornent
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField] 
    private SushiComponent sushiComponent = default;

    [SerializeField, Header("プレイヤーの大きさ")]
    private float playerSize = 1.3f; // プレイヤーの大きさ

    private void Reset()
    {
        // Rigidbodyの初期化
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

    }

    public override void GimmickMoveDorifutoWall( DorifutoWallGimmickPlay gimmickDat )
    {
        float distance = ( playerSize + gimmickDat.BasicWidth / 2 ) * gimmickDat.HitRight; // ドリフトウォールの接触位置からの距離を計算

        if (sushiComponent.SplineAnimateRigidbody.PlayFromClosestPoint( gimmickDat.SplineContainer, rb, distance ))
        {
            sushiComponent.SetSushiMode( SushiMode.DriftWall ); // ドリフトウォールモードに設定
            return;
        }
    }
}
