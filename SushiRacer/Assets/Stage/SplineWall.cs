using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Unity.Mathematics; // 追加

[StructLayout( LayoutKind.Sequential )]
struct VertexData
{
    public Vector3 Position;
}

[RequireComponent( typeof( SplineContainer ), typeof( MeshFilter ), typeof( MeshCollider ) )]
public class SplineWall : MonoBehaviour
{
    [SerializeField, Tooltip( "スプラインにメッシュを合わせるか" )]
    private bool updating = true;

    // 追加: Y 座標を均一化するための目標値
    [SerializeField, Tooltip( "各スプラインの Y 座標を均一化する値" )]
    private float uniformY = 0f;

    [SerializeField, Range( 0.1f, 100f ), Tooltip( "壁の頂点間の長さ" )]
    private float segmentLength = 1.0f;

    [SerializeField, Tooltip( "壁の高さ" )]
    private float height = 5.0f;

    [SerializeField, ReadOnly]
    private SplineContainer splineContainer;
    [SerializeField, ReadOnly]
    private Mesh mesh;
    [SerializeField, ReadOnly]
    private MeshFilter meshFilter;
    [SerializeField, ReadOnly]
    private MeshCollider meshCollider;

    [SerializeField, Range( -10f, 10f )]
    private float expandFactor = 0.5f; // メッシュを拡縮する割合

    [SerializeField, Range( 200, 1000 )]
    private int splinePointReduction = 1000; // スプラインポイントの減点数

    private float cachedLength = -1f;

    private void Awake()
    {
        // メッシュの独立性を確保
        updating = false;

        PrepareComponents();
        Rebuild();
    }

    private void Reset()
    {
        PrepareComponents();
        Rebuild();
    }

    private void PrepareComponents()
    {
        if (!TryGetComponent( out splineContainer ))
        {
            Debug.LogError( "SplineContainer コンポーネントが見つかりません。" );
        }

        if (!TryGetComponent( out meshFilter ))
        {
            Debug.LogError( "MeshFilter コンポーネントが見つかりません。" );
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }
        else if (meshFilter.sharedMesh != mesh)
        {
            // 他のオブジェクトとメッシュを共有している場合、新しいメッシュを作成
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }

        if (!TryGetComponent( out meshCollider ))
        {
            Debug.LogError( "MeshCollider コンポーネントが見つかりません。" );
        }
        else
        {
            meshCollider.sharedMesh = mesh;
        }
    }


    public void Rebuild()
    {
        if (!updating)
        {
            return;
        }

        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "Spline が設定されていません。" );
            return;
        }

        // スプラインの総長を取得（キャッシュを使用）
        float totalLength = GetSplineLength( splineContainer.Spline );

        // divided を計算
        int divided = Mathf.Max( 2, Mathf.CeilToInt( totalLength / segmentLength ) );

        mesh.Clear();

        try
        {
            var meshDataArray = Mesh.AllocateWritableMeshData( 1 );
            var meshData = meshDataArray[0];
            meshData.subMeshCount = 1;

            int vertexCount = 2 * ( divided + 1 );
            int indexCount = 6 * divided;

            meshData.SetIndexBufferParams( indexCount, IndexFormat.UInt32 );
            meshData.SetVertexBufferParams( vertexCount, new VertexAttributeDescriptor[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position),
            } );

            var vertices = meshData.GetVertexData<VertexData>();
            var indices = meshData.GetIndexData<UInt32>();
            for (int i = 0; i <= divided; ++i)
            {
                float t = (float)i / divided;
                // スプラインから位置を評価し、Y 座標は無視してXZ成分のみを残す
                splineContainer.Spline.Evaluate( t, out var pos, out _, out _ );
                Vector3 position = new Vector3( pos.x, uniformY, pos.z );

                // 下側の頂点を設定
                var vertex0 = vertices[2 * i];
                vertex0.Position = position;
                vertices[2 * i] = vertex0;

                // 上側の頂点は uniformY + height とする
                var vertex1 = vertices[2 * i + 1];
                Vector3 posUpper = position;
                posUpper.y += height;
                vertex1.Position = posUpper;
                vertices[2 * i + 1] = vertex1;
            }


            for (int i = 0; i < divided; ++i)
            {
                int baseIndex = 6 * i;
                int vertIndex = 2 * i;
                indices[baseIndex + 0] = (UInt32)( vertIndex + 0 );
                indices[baseIndex + 1] = (UInt32)( vertIndex + 1 );
                indices[baseIndex + 2] = (UInt32)( vertIndex + 2 );
                indices[baseIndex + 3] = (UInt32)( vertIndex + 1 );
                indices[baseIndex + 4] = (UInt32)( vertIndex + 3 );
                indices[baseIndex + 5] = (UInt32)( vertIndex + 2 );
            }

            meshData.SetSubMesh( 0, new SubMeshDescriptor( 0, indexCount ) );
            Mesh.ApplyAndDisposeWritableMeshData( meshDataArray, mesh );

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // コライダーを更新
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError( $"メッシュの再構築中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}" );
            return;
        }

        // expandFactor による内側拡縮を適用
        ApplyExpandFactor();
    }

    private void ApplyExpandFactor()
    {
        if (mesh == null)
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        // メッシュの独立性を確保するために新しいメッシュを生成
        Mesh expandedMesh = Instantiate( mesh );
        expandedMesh.name = $"{gameObject.name}_ExpandedMesh";
        meshFilter.mesh = expandedMesh;

        Vector3[] vertices = expandedMesh.vertices;
        Vector3[] normals = expandedMesh.normals;

        for (int i = 0; i < vertices.Length; i++)
        {
            // 法線から Y 成分を除去し、XZ平面上の方向を得る
            Vector3 n = normals[i];
            n.y = 0f;
            if (n.sqrMagnitude > 0.0001f)
            {
                n.Normalize();
            }
            // XZ 平面だけで拡縮を適用
            vertices[i] += n * expandFactor;
        }

        expandedMesh.vertices = vertices;
        expandedMesh.RecalculateBounds();
        expandedMesh.RecalculateNormals();

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = expandedMesh;
        }

        mesh = expandedMesh;
    }

    /// <summary>
    /// スプライン内の各 Knot の Y 座標を uniformY の値に均一化します。
    /// </summary>
    public void UniformizeSplineY()
    {
        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "スプラインが設定されていません。" );
            return;
        }

        int knotCount = splineContainer.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            BezierKnot knot = (BezierKnot)splineContainer.Spline[i];
            Vector3 pos = knot.Position;
            pos.y = uniformY;
            knot.Position = pos;
            splineContainer.Spline[i] = knot;
        }
        Debug.Log( $"スプラインのY座標が {uniformY} に均一化されました。" );
    }

    /// <summary>
    /// スプライン内の各 Knot の回転の X, Z をリセットし、Y 軸の値のみ残します。
    /// </summary>
    public void ResetSplineRotation()
    {
        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "スプラインが設定されていません。" );
            return;
        }

        int knotCount = splineContainer.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            // BezierKnot 型であると仮定
            BezierKnot knot = (BezierKnot)splineContainer.Spline[i];

            // math.quaternion を UnityEngine.Quaternion に変換して Euler 角を取得
            UnityEngine.Quaternion unityQuat = new UnityEngine.Quaternion( knot.Rotation.value.x, knot.Rotation.value.y, knot.Rotation.value.z, knot.Rotation.value.w );
            float yRotationDegrees = unityQuat.eulerAngles.y;
            // EulerXYZ() はラジアンを想定しているため変換
            float yRotationRadians = math.radians( yRotationDegrees );

            // X, Z を 0 にして Y 軸のみ残す新たな回転を設定
            knot.Rotation = quaternion.EulerXYZ( new float3( 0f, yRotationRadians, 0f ) );
            splineContainer.Spline[i] = knot;
        }
        Debug.Log( "スプラインの回転がY軸だけ残す形でリセットされました。" );
    }


    public void GenerateSplineFromMesh()
    {
        if (mesh == null)
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        if (mesh.vertexCount % 2 != 0)
        {
            Debug.LogError( "メッシュの頂点数が不正です。頂点数は偶数である必要があります。" );
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int totalPoints = vertices.Length / 2;

        splinePointReduction = Mathf.Clamp( splinePointReduction, 1, totalPoints );

        int pointCount = Mathf.CeilToInt( (float)totalPoints / splinePointReduction );
        splineContainer.Spline.Clear();

        for (int i = 0; i < totalPoints; i += splinePointReduction)
        {
            Vector3 position = vertices[2 * i];
            splineContainer.Spline.Add( new BezierKnot( position, 0f, 0f, Quaternion.identity ) );
        }

        if (( totalPoints - 1 ) % splinePointReduction != 0)
        {
            Vector3 lastPosition = vertices[2 * ( totalPoints - 1 )];
            splineContainer.Spline.Add( new BezierKnot( lastPosition, 0f, 0f, Quaternion.identity ) );
        }

        Debug.Log( "メッシュからスプラインを生成しました。" );
    }

    private float GetSplineLength( Spline spline )
    {
        if (cachedLength < 0f)
        {
            cachedLength = CalculateSplineLength( spline );
        }
        return cachedLength;
    }

    private float CalculateSplineLength( Spline spline )
    {
        float length = 0f;
        const int sampleCount = 100;
        Vector3 previousPoint = spline.EvaluatePosition( 0f );
        for (int i = 1; i <= sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            Vector3 currentPoint = spline.EvaluatePosition( t );
            length += Vector3.Distance( previousPoint, currentPoint );
            previousPoint = currentPoint;
        }
        return length;
    }

    public void ReverseMeshFaces()
    {
        if (mesh == null)
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        mesh = Instantiate( mesh );
        mesh.name = $"{gameObject.name}_ReversedMesh";
        meshFilter.mesh = mesh;

        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        updating = false;
    }

    private void OnValidate()
    {
        if (meshFilter == null)
        {
            TryGetComponent( out meshFilter );
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh;
        }

        if (meshFilter != null && meshFilter.sharedMesh != mesh)
        {
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh;
        }
    }
}
