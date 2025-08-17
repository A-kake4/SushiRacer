using UnityEngine;
using UnityEngine.Splines;

public class DorifutoWallGimmickPlay : BaseGimmickPlayCompornent
{
    [SerializeField, Header( "ドリフトウォールの移動コンポーネント" )]
    private SplineContainer splineContainer = default;
    public SplineContainer SplineContainer
    {
        get { return splineContainer; }
    }

    [SerializeField, Header( "ドリフトウォールの基本幅" )]
    private float basicWidth = 0.5f; // ドリフトウォールの基本幅（必要に応じて調整）
    public float BasicWidth
    {
        get { return basicWidth; }
    }

    private int hitRight = 1; // 右側の接触
    public int HitRight
    {
        get { return hitRight; }
    }

    private void Reset()
    {
        // ドリフトウォールの移動コンポーネントの初期化
        splineContainer = GetComponent<SplineContainer>();
    }

    public override void GimmickPlayTriggerEnter( BaseGimmickMoveCompornent hitObject )
    {
        // 接触位置としてhitObjectの位置を利用（必要に応じて修正）
        Vector3 contactPoint = hitObject.transform.position;
        hitRight = DetermineContactSide( splineContainer, contactPoint );

        hitObject.GimmickMoveDorifutoWall( this );
    }

    // スプライン上で接触点に最も近いパラメータを求めます。
    private float FindClosestParameter( Spline spline, Vector3 point )
    {
        float closestParam = 0f;
        float minDist = float.MaxValue;
        const int samples = 100;
        for (int i = 0; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 samplePos = spline.EvaluatePosition( t );
            float dist = Vector3.Distance( point, samplePos );
            if (dist < minDist)
            {
                minDist = dist;
                closestParam = t;
            }
        }
        return closestParam;
    }

    // 接触点がスプライン上のどちら側かを判断します。
    // 戻り値: "Right", "Left", "Center", "Unknown"
    private int DetermineContactSide( SplineContainer splineContainer, Vector3 contactPoint )
    {
        if (splineContainer == null || splineContainer.Spline == null)
            return 0;

        float t = FindClosestParameter( splineContainer.Spline, contactPoint );
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition( t );
        // float3をVector3に変換してから正規化
        Vector3 tangent = ( (Vector3)splineContainer.Spline.EvaluateTangent( t ) ).normalized;
        Vector3 diff = ( contactPoint - splinePos ).normalized;

        // 接線と接触方向の外積の上方向成分で左右判定
        float crossY = Vector3.Dot( Vector3.Cross( tangent, diff ), Vector3.up );
        if (crossY > 0)
            return 1;
        else
            return -1;
    }
}