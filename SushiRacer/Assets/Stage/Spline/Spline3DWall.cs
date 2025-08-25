using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;

[RequireComponent( typeof( SplineContainer ), typeof( MeshFilter ), typeof( MeshCollider ) )]
public class Spline3DWall : MonoBehaviour
{
    [SerializeField, Tooltip( "スプラインにメッシュを合わせるか" )]
    private bool updating = true;

    [SerializeField, Range( 0.1f, 100f ), Tooltip( "壁の頂点間の長さ" )]
    private float segmentLength = 1.0f;

    [SerializeField, Tooltip( "壁の高さ" )]
    private float height = 5.0f;

    [SerializeField, Range( 0.1f, 10f ), Tooltip( "壁の厚さ" )]
    private float thickness = 1.0f;

    [SerializeField, ReadOnly]
    private SplineContainer splineContainer;
    [SerializeField, ReadOnly]
    private Mesh mesh;
    [SerializeField, ReadOnly]
    private MeshFilter meshFilter;
    [SerializeField, ReadOnly]
    private MeshCollider meshCollider;

    //[SerializeField, Tooltip( "メッシュの面を反転するかどうか" )]
    private bool reverseFaces = false;

    private float cachedLength = -1f;

    public SplineContainer SplineContainerField => splineContainer;

    private void Awake()
    {
        // メッシュの独立性のため、updating を false に設定
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
            mesh = new Mesh { name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh" };
            meshFilter.mesh = mesh;
        }
        else if (meshFilter.sharedMesh != mesh)
        {
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh";
            meshFilter.mesh = mesh;
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
            return;

        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "Spline が設定されていません。" );
            return;
        }

        // 毎回新規の Mesh を作成（共有状態を防止）
        mesh = new Mesh { name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh" };
        meshFilter.mesh = mesh;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }

        float totalLength = GetSplineLength( splineContainer.Spline );
        int divided = Mathf.Max( 2, Mathf.CeilToInt( totalLength / segmentLength ) );
        bool closed = splineContainer.Spline.Closed;

        mesh.Clear();

        try
        {
            var meshDataArray = Mesh.AllocateWritableMeshData( 1 );
            var meshData = meshDataArray[0];

            // 各サンプル点につき4頂点（断面四角形）を生成
            int vertexCount = 4 * ( divided + 1 );
            // 各セグメントにおいて、断面の各辺からクワッド（2三角形、計6頂点）を生成
            int sideIndices = 4 * divided * 6;
            // スプラインが閉じていなければエンドキャップを追加（開始と終了それぞれ6頂点）
            int capIndices = closed ? 0 : 2 * 6;
            int indexCount = sideIndices + capIndices;

            meshData.subMeshCount = 1;
            meshData.SetIndexBufferParams( indexCount, IndexFormat.UInt32 );
            meshData.SetVertexBufferParams( vertexCount, new VertexAttributeDescriptor[]
            {
            new VertexAttributeDescriptor(VertexAttribute.Position),
            } );

            var vertices = meshData.GetVertexData<VertexData>();
            var indices = meshData.GetIndexData<UInt32>();

            float halfThickness = thickness * 0.5f;
            VertexData[] vertexArray = new VertexData[vertexCount];
            Vector3[] firstProfile = new Vector3[4];

            // 各サンプル点に対して断面（bottom front, top front, top back, bottom back）の頂点を生成
            for (int i = 0; i <= divided; i++)
            {
                float t = (float)i / divided;
                splineContainer.Spline.Evaluate( t, out var pos, out var tangent, out _ );

                // タンジェントの水平成分で法線計算
                Vector3 tangentXZ = new Vector3( tangent.x, 0f, tangent.z );
                if (tangentXZ == Vector3.zero)
                    tangentXZ = Vector3.forward;
                tangentXZ.Normalize();

                float3 normal = Vector3.Cross( Vector3.up, tangentXZ ).normalized;

                Vector3 bottomFront = pos + normal * halfThickness;
                Vector3 topFront = bottomFront + Vector3.up * height;
                Vector3 bottomBack = pos - normal * halfThickness;
                Vector3 topBack = bottomBack + Vector3.up * height;

                int baseIndex = i * 4;
                vertexArray[baseIndex + 0].Position = bottomFront;
                vertexArray[baseIndex + 1].Position = topFront;
                vertexArray[baseIndex + 2].Position = topBack;
                vertexArray[baseIndex + 3].Position = bottomBack;

                if (i == 0)
                {
                    firstProfile[0] = bottomFront;
                    firstProfile[1] = topFront;
                    firstProfile[2] = topBack;
                    firstProfile[3] = bottomBack;
                }
                else if (closed && i == divided)
                {
                    // 閉じたスプラインの場合、最終断面を開始断面と一致させる
                    int lastBase = i * 4;
                    vertexArray[lastBase + 0].Position = firstProfile[0];
                    vertexArray[lastBase + 1].Position = firstProfile[1];
                    vertexArray[lastBase + 2].Position = firstProfile[2];
                    vertexArray[lastBase + 3].Position = firstProfile[3];
                }
            }

            // 頂点の書き込み
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = vertexArray[i];
            }

            int indexOffset = 0;
            // 各セグメント間の断面を連結して側面を生成
            // 断面内の頂点の順番: 0: bottom front, 1: top front, 2: top back, 3: bottom back.
            for (int i = 0; i < divided; i++)
            {
                int curr = i * 4;
                int next = ( i + 1 ) * 4;

                // Front face
                indices[indexOffset++] = (uint)( curr + 0 );
                indices[indexOffset++] = (uint)( curr + 1 );
                indices[indexOffset++] = (uint)( next + 0 );

                indices[indexOffset++] = (uint)( curr + 1 );
                indices[indexOffset++] = (uint)( next + 1 );
                indices[indexOffset++] = (uint)( next + 0 );

                // Top face
                indices[indexOffset++] = (uint)( curr + 1 );
                indices[indexOffset++] = (uint)( curr + 2 );
                indices[indexOffset++] = (uint)( next + 1 );

                indices[indexOffset++] = (uint)( curr + 2 );
                indices[indexOffset++] = (uint)( next + 2 );
                indices[indexOffset++] = (uint)( next + 1 );

                // Back face
                indices[indexOffset++] = (uint)( curr + 2 );
                indices[indexOffset++] = (uint)( curr + 3 );
                indices[indexOffset++] = (uint)( next + 2 );

                indices[indexOffset++] = (uint)( curr + 3 );
                indices[indexOffset++] = (uint)( next + 3 );
                indices[indexOffset++] = (uint)( next + 2 );

                // Bottom face
                indices[indexOffset++] = (uint)( curr + 3 );
                indices[indexOffset++] = (uint)( curr + 0 );
                indices[indexOffset++] = (uint)( next + 3 );

                indices[indexOffset++] = (uint)( curr + 0 );
                indices[indexOffset++] = (uint)( next + 0 );
                indices[indexOffset++] = (uint)( next + 3 );
            }

            // スプラインが閉じていない場合、開始と終了にキャップを追加
            if (!closed)
            {
                // 開始キャップ（最初の断面）のキャップ
                indices[indexOffset++] = 0;
                indices[indexOffset++] = 2;
                indices[indexOffset++] = 1;

                indices[indexOffset++] = 0;
                indices[indexOffset++] = 3;
                indices[indexOffset++] = 2;

                // 終了キャップ（最後の断面）のキャップ
                int lastBase = divided * 4;
                indices[indexOffset++] = (uint)( lastBase + 0 );
                indices[indexOffset++] = (uint)( lastBase + 1 );
                indices[indexOffset++] = (uint)( lastBase + 2 );

                indices[indexOffset++] = (uint)( lastBase + 0 );
                indices[indexOffset++] = (uint)( lastBase + 2 );
                indices[indexOffset++] = (uint)( lastBase + 3 );
            }

            meshData.SetSubMesh( 0, new SubMeshDescriptor( 0, indexCount ) );
            Mesh.ApplyAndDisposeWritableMeshData( meshDataArray, mesh );

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }

            if (reverseFaces)
            {
                ReverseMeshFaces();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError( $"メッシュ再構築中にエラー: {ex.Message}\n{ex.StackTrace}" );
            return;
        }
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
        mesh.name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh";
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
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // エディター実行中の場合、遅延呼び出しで実行
            EditorApplication.delayCall += () =>
            {
                // オブジェクトが既に破棄されていないか確認
                if (this == null)
                    return;
                ValidateMeshFilter();
            };
        }
        else
        {
            ValidateMeshFilter();
        }
#endif
    }

    private void ValidateMeshFilter()
    {
        if ( meshFilter == null )
        {
            TryGetComponent( out meshFilter );
        }

#if UNITY_EDITOR
        // エディター実行中（プレイモードでない場合）は sharedMesh を用いる
        if ( !Application.isPlaying )
        {
            if ( mesh == null )
            {
                mesh = new Mesh { name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh" };
                meshFilter.sharedMesh = mesh;
            }
            else if ( meshFilter.sharedMesh != mesh )
            {
                mesh = Instantiate( meshFilter.sharedMesh );
                mesh.name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh";
                meshFilter.sharedMesh = mesh;
            }
            return;
        }
#endif
        // プレイ中の場合は通常どおり mesh プロパティを用いる
        if ( mesh == null )
        {
            mesh = new Mesh { name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh" };
            meshFilter.mesh = mesh;
        }
        else if ( meshFilter.sharedMesh != mesh )
        {
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_3dWallMesh";
            meshFilter.mesh = mesh;
        }
    }
}