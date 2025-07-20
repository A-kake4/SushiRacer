using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[StructLayout( LayoutKind.Sequential )]
struct VertexData
{
    public Vector3 Position;
}

[RequireComponent( typeof( SplineContainer ), typeof( MeshFilter ), typeof( MeshCollider ) )]
public class SplineWall : MonoBehaviour
{
    [SerializeField] private bool updating = true;

    [SerializeField, Range(0.1f, 100f)] private float segmentLength = 1.0f;
    [SerializeField] private float height = 5.0f;

    [SerializeField, ReadOnly]
    private SplineContainer splineContainer;
    [SerializeField, ReadOnly]
    private Mesh mesh;
    [SerializeField, ReadOnly]
    private MeshFilter meshFilter;
    [SerializeField, ReadOnly]
    private MeshCollider meshCollider;

    [SerializeField, Range(-1f, 1f)]
    private float expandFactor = 0.5f; // メッシュを拡縮する割合

    [SerializeField, Range(200, 1000)]
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
        if ( !TryGetComponent( out splineContainer ) )
        {
            Debug.LogError( "SplineContainer コンポーネントが見つかりません。" );
        }

        if ( !TryGetComponent( out meshFilter ) )
        {
            Debug.LogError( "MeshFilter コンポーネントが見つかりません。" );
        }

        if ( mesh == null )
        {
            mesh = new Mesh();

            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }
        else if ( meshFilter.sharedMesh != mesh )
        {
            // 他のオブジェクトとメッシュを共有している場合、新しいメッシュを作成
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }

        if ( !TryGetComponent( out meshCollider ) )
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
        if(!updating)
        {
            return;
        }

        if ( splineContainer?.Spline == null )
        {
            Debug.LogWarning( "Spline が設定されていません。" );
            return;
        }

        // スプラインの総長を取得（キャッシュを使用）
        float totalLength = GetSplineLength(splineContainer.Spline);

        // divided を計算
        int divided = Mathf.Max(2, Mathf.CeilToInt(totalLength / segmentLength));

        mesh.Clear();

        try
        {
            // 'using' ブロックを削除
            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArray[0];
            meshData.subMeshCount = 1;

            int vertexCount = 2 * (divided + 1);
            int indexCount = 6 * divided;

            meshData.SetIndexBufferParams( indexCount, IndexFormat.UInt32 );
            meshData.SetVertexBufferParams( vertexCount, new VertexAttributeDescriptor[]
            {
            new VertexAttributeDescriptor(VertexAttribute.Position),
            } );

            var vertices = meshData.GetVertexData<VertexData>();
            var indices = meshData.GetIndexData<UInt32>();

            for ( int i = 0; i <= divided; ++i )
            {
                float t = (float)i / divided;
                splineContainer.Spline.Evaluate( t, out var position, out _, out _ );

                // 一時変数に代入してから設定
                var vertex0 = vertices[2 * i];
                vertex0.Position = position;
                vertices[2 * i] = vertex0;

                var vertex1 = vertices[2 * i + 1];

                position.y += height;

                vertex1.Position = position;
                vertices[2 * i + 1] = vertex1;
            }

            for ( int i = 0; i < divided; ++i )
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
            Mesh.ApplyAndDisposeWritableMeshData( meshDataArray, mesh ); // 自動でDisposeされるため、ここでのDisposeは不要

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // コライダーを更新
            if ( meshCollider != null )
            {
                meshCollider.sharedMesh = null; // 既存のコライダーをクリア
                meshCollider.sharedMesh = mesh; // 新しいメッシュを設定
            }
        }
        catch ( Exception ex )
        {
            Debug.LogError( $"メッシュの再構築中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}" );
        }
    }



    public void GenerateSplineFromMesh()
    {
        if ( mesh == null )
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        // メッシュの頂点数が偶数であることを確認
        if ( mesh.vertexCount % 2 != 0 )
        {
            Debug.LogError( "メッシュの頂点数が不正です。頂点数は偶数である必要があります。" );
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int totalPoints = vertices.Length / 2;

        // スプラインポイントの減点数が有効範囲内であることを確認
        splinePointReduction = Mathf.Clamp( splinePointReduction, 1, totalPoints );

        // 減点数に基づいてポイント数を計算
        int pointCount = Mathf.CeilToInt((float)totalPoints / splinePointReduction);

        // 既存のスプラインをクリア
        splineContainer.Spline.Clear();

        for ( int i = 0; i < totalPoints; i += splinePointReduction )
        {
            Vector3 position = vertices[2 * i];

            // スプラインポイントの追加
            splineContainer.Spline.Add( new BezierKnot( position, 0f, 0f, Quaternion.identity ) );
        }

        // 最後のポイントがスプラインに含まれていない場合、追加
        if ( ( totalPoints - 1 ) % splinePointReduction != 0 )
        {
            Vector3 lastPosition = vertices[2 * (totalPoints - 1)];
            splineContainer.Spline.Add( new BezierKnot( lastPosition, 0f, 0f, Quaternion.identity ) );
        }

        // スプラインを更新
        //splineContainer.Spline.ComputeTangents();
        //splineContainer.RefreshAll();

        Debug.Log( "メッシュからスプラインを生成しました。" );
    }



    /// <summary>
    /// スプラインの総長を計算します。キャッシュを利用してパフォーマンスを向上させます。
    /// </summary>
    /// <param name="spline">対象のスプライン</param>
    /// <returns>スプラインの総長</returns>
    private float GetSplineLength( Spline spline )
    {
        if ( cachedLength < 0f )
        {
            cachedLength = CalculateSplineLength( spline );
        }
        return cachedLength;
    }

    /// <summary>
    /// スプラインの総長を計算します。
    /// </summary>
    /// <param name="spline">対象のスプライン</param>
    /// <returns>スプラインの総長</returns>
    private float CalculateSplineLength( Spline spline )
    {
        float length = 0f;
        const int sampleCount = 100;
        Vector3 previousPoint = spline.EvaluatePosition(0f);
        for ( int i = 1; i <= sampleCount; i++ )
        {
            float t = (float)i / sampleCount;
            Vector3 currentPoint = spline.EvaluatePosition(t);
            length += Vector3.Distance( previousPoint, currentPoint );
            previousPoint = currentPoint;
        }
        return length;
    }

    /// <summary>
    /// メッシュの面を反対にします（法線を反転）。
    /// </summary>
    public void ReverseMeshFaces()
    {
        if ( mesh == null )
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        // メッシュの独立性を確保するために新しいメッシュを作成
        mesh = Instantiate( mesh );
        mesh.name = $"{gameObject.name}_ReversedMesh";
        meshFilter.mesh = mesh;

        var triangles = mesh.triangles;
        for ( int i = 0; i < triangles.Length; i += 3 )
        {
            // 頂点の順序を逆にする
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // コライダーを更新
        if ( meshCollider != null )
        {
            meshCollider.sharedMesh = null; // 既存のコライダーをクリア
            meshCollider.sharedMesh = mesh; // 新しいメッシュを設定
        }

        updating = false; // 更新フラグをリセット
    }

    public void ExpandMeshInnerSide()
    {
        if ( mesh == null )
        {
            Debug.LogError( "メッシュが存在しません。" );
            return;
        }

        // メッシュの独立性を確保するために新しいメッシュを作成
        mesh = Instantiate( mesh );
        mesh.name = $"{gameObject.name}_ExpandedMesh";
        meshFilter.mesh = mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for ( int i = 0; i < vertices.Length; i++ )
        {
            vertices[i] += normals[i] * expandFactor;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // コライダーを更新
        if ( meshCollider != null )
        {
            meshCollider.sharedMesh = null; // 既存のコライダーをクリア
            meshCollider.sharedMesh = mesh; // 新しいメッシュを設定
        }

        updating = false; // 更新フラグをリセット
    }


    /// <summary>
    /// エディター上での変更を検出し、メッシュの独立性を確保します。
    /// </summary>
    private void OnValidate()
    {
        if ( meshFilter == null )
        {
            TryGetComponent( out meshFilter );
        }

        if ( mesh == null )
        {
            mesh = new Mesh();
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }

        if ( meshFilter != null && meshFilter.sharedMesh != mesh )
        {
            // 他のオブジェクトとメッシュを共有している場合、新しいメッシュを作成
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }
    }
}
