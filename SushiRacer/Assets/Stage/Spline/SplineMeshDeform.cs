using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMeshDeform : MonoBehaviour
{
    // スプライン情報を格納するコンポーネント
    [SerializeField] private SplineContainer _container;

    // スプライン上の位置を示すパラメータ（0～1の範囲で指定）
    [SerializeField, Range(0, 1)] private float _t;

    // メッシュの前方向の軸
    [SerializeField] private Vector3 _forwardAxis = Vector3.forward;

    // メッシュの上方向の軸
    [SerializeField] private Vector3 _upAxis = Vector3.up;

    private bool _isStarted;

    // 変形対象のメッシュ
    private MeshFilter _meshFilter;

    // メッシュコライダー（オプション）
    private MeshCollider _meshCollider;

    // 変形されたメッシュ情報
    private Mesh _deformedMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _originalNormals;

    // メッシュを変形する
    public void Deform()
    {
        if ( _container == null || _meshFilter == null )
            return;

        if ( _deformedMesh == null )
        {
            // メッシュ情報のコピー
            _deformedMesh = _meshFilter.mesh;
            _originalVertices = _deformedMesh.vertices;
            _originalNormals = _deformedMesh.normals;
        }

        // 元軸からの回転クォータニオンを計算
        var axisRotation = Quaternion.Inverse(Quaternion.LookRotation(_forwardAxis, _upAxis));

        // スプラインのローカル座標からメッシュのローカル座標に変換する行列
        // スプラインのローカル座標→ワールド座標→メッシュのローカル座標の順で変換する必要がある
        var splineToLocalMatrix = transform.worldToLocalMatrix * _container.transform.localToWorldMatrix;

        // メッシュの頂点情報を一時的に格納する配列
        NativeArray<float3> deformedVertices = default;
        NativeArray<float3> deformedNormals = default;

        try
        {
            // 配列の確保
            deformedVertices = new NativeArray<float3>( _originalVertices.Length, Allocator.Temp );
            deformedNormals = new NativeArray<float3>( _originalNormals.Length, Allocator.Temp );

            // スプラインの情報を格納するNativeSplineを作成
            using var spline = new NativeSpline(_container.Spline, splineToLocalMatrix);

            // スプラインの長さを計算
            var splineLength = spline.CalculateLength(splineToLocalMatrix);

            // メッシュの頂点情報をスプラインに沿って変形
            for ( var i = 0; i < deformedVertices.Length; i++ )
            {
                // 元の頂点座標
                var originalVertex = _originalVertices[i];

                // 軸の回転補正
                originalVertex = math.mul( axisRotation, originalVertex );

                // 頂点座標の前方向成分をスプライン上の位置に変換
                var t = originalVertex.z / splineLength + _t;

                // 囲まれたスプラインなら、tの値をループさせる
                if ( spline.Closed ) t = Mathf.Repeat( t, 1 );

                // 計算されたtに置ける位置・接線・上向きベクトルを取得
                spline.Evaluate(
                    t,
                    out var splinePos,
                    out var splineTangent,
                    out var splineUp
                );

                // スプラインに対するオフセットの回転クォータニオン
                var rotation = quaternion.LookRotationSafe(splineTangent, splineUp);

                // スプライン上の位置に対して水平垂直なずらし位置を計算
                var offset = math.mul(rotation, new float3(originalVertex.x, originalVertex.y, 0));

                // スプライン位置に対する頂点座標を計算
                deformedVertices[i] = splinePos + offset;

                // スプライン位置に対する法線ベクトルを計算
                // 法線にも軸の回転補正を適用
                var normal = math.mul(axisRotation, _originalNormals[i]);
                deformedNormals[i] = math.mul( rotation, normal );
            }

            // メッシュ情報を更新
            _deformedMesh.SetVertices( deformedVertices );
            _deformedMesh.SetNormals( deformedNormals );

            // バウンディングボックスの再計算
            _deformedMesh.RecalculateBounds();

            // メッシュコライダーの更新
            if ( _meshCollider != null )
                _meshCollider.sharedMesh = _deformedMesh;
        }
        finally
        {
            // メモリ解放
            if ( deformedVertices.IsCreated ) deformedVertices.Dispose();
            if ( deformedNormals.IsCreated ) deformedNormals.Dispose();
        }
    }

    // 初期化処理
    private void Start()
    {
        _isStarted = true;

        TryGetComponent( out _meshFilter );
        TryGetComponent( out _meshCollider );

        // 初回に変形を行う
        Deform();
    }

    // 後処理
    private void OnDestroy()
    {
        // MeshFilterから取得したメッシュは明示的に破棄する必要がある
        if ( _deformedMesh != null )
            Destroy( _deformedMesh );

        _deformedMesh = null;
    }

#if UNITY_EDITOR
    // インスペクターから操作された際に変形を反映
    private void OnValidate()
    {
        // 開始前にOnValidateが呼ばれることがあるため、_isStartedを確認
        if ( !_isStarted ) return;

        Deform();
    }
#endif
}