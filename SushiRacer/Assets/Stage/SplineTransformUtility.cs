using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent( typeof( SplineContainer ))]
public class SplineTransformUtility : MonoBehaviour
{
    public float uniformY = 0f; // スプラインのY座標を均一化するための値

    /// <summary>
    /// 指定されたスプラインコンテナ内の各 Knot の回転のX, Zをリセットし、Y軸のみ残すように調整します。
    /// </summary>
    public void ResetSplineRotation()
    {
        if (!TryGetValidSplineContainer( out SplineContainer container ))
            return;

        int knotCount = container.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            BezierKnot knot = (BezierKnot)container.Spline[i];

            // 現在のRotationをQuaternionに変換し、Y軸の値のみを取得
            Quaternion unityQuat = new Quaternion(
                knot.Rotation.value.x,
                knot.Rotation.value.y,
                knot.Rotation.value.z,
                knot.Rotation.value.w );
            float yRotationDegrees = unityQuat.eulerAngles.y;
            float yRotationRadians = math.radians( yRotationDegrees );

            // Y軸のみを反映する回転に変更
            knot.Rotation = quaternion.EulerXYZ( new float3( 0f, yRotationRadians, 0f ) );
            container.Spline[i] = knot;
        }

        Debug.Log( "スプラインの回転がY軸のみを保持する形でリセットされました。" );
    }

    /// <summary>
    /// 指定されたスプラインコンテナ内の各 Knot のY座標を uniformY の値に統一します。
    /// </summary>
    public void UniformizeSplineY()
    {
        if (!TryGetValidSplineContainer( out SplineContainer container ))
            return;

        int knotCount = container.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            BezierKnot knot = (BezierKnot)container.Spline[i];
            Vector3 pos = knot.Position;
            pos.y = uniformY;
            knot.Position = pos;
            container.Spline[i] = knot;
        }
        Debug.Log( $"スプラインのY座標が {uniformY} に均一化されました。" );
    }

    /// <summary>
    /// SplineContainer と有効なスプラインが存在するかをチェックします。
    /// </summary>
    /// <param name="container">取得されたSplineContainer</param>
    /// <returns>有効な場合はtrue、そうでなければfalse</returns>
    private bool TryGetValidSplineContainer( out SplineContainer container )
    {
        container = GetComponent<SplineContainer>();
        if (container == null)
        {
            Debug.LogWarning( "SplineContainerがアタッチされていません。" );
            return false;
        }

        if (container.Spline == null)
        {
            Debug.LogWarning( "スプラインが設定されていません。" );
            return false;
        }
        return true;
    }
}
