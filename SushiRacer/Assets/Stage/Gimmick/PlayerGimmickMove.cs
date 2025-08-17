using UnityEngine;
using UnityEngine.Splines;

public class PlayerGimmickMove : BaseGimmickMoveCompornent
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField] 
    private SushiComponent sushiComponent = default;

    //[SerializeField]
    //private SpinImput spinImput = default;

    [SerializeField, Header("ドリフトウォールの移動コンポーネント")]
    private SplineAnimateRigidbody splineAnimateRigidbody = default;


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
        if (splineAnimateRigidbody == null)
        {
            return;
        }
        sushiComponent.SetSushiMode( SushiMode.DriftWall ); // ドリフトウォールモードに設定
        sushiComponent.SplineAnimateRigidbody = splineAnimateRigidbody; // スプラインアニメーションの設定

        float distance = (1.3f + gimmickDat.BasicWidth / 2 ) * gimmickDat.HitRight; // ドリフトウォールの接触位置からの距離を計算

        splineAnimateRigidbody.PlayFromClosestPoint( gimmickDat.SplineContainer, rb, distance );
    }
}
