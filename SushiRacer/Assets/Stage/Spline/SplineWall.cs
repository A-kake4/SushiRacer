using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
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
    [SerializeField, Tooltip( "スプラインにメッシュを合わせるか" )]
    private bool updating = true;

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

    // メッシュの面反転を行うかどうかのフラグ【追加】
    [SerializeField, Tooltip( "メッシュの面を反転するかどうか" )]
    private bool reverseFaces = false;

    private float cachedLength = -1f;

    // 外部から SplineContainer にアクセスできるプロパティ
    public SplineContainer SplineContainerField => splineContainer;
    // インスペクタで設定可能な uniformY の値にアクセスするプロパティ
    //public float UniformYValue => uniformY;

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
            mesh = new Mesh { name = $"{gameObject.name}_Mesh" };
            meshFilter.mesh = mesh; // sharedMesh ではなく mesh を使用
        }
        else if (meshFilter.sharedMesh != mesh)
        {
            // 他のオブジェクトとメッシュを共有している場合、新しいメッシュを作成
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
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

        float totalLength = GetSplineLength( splineContainer.Spline );
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

            Vector3 firstOffsetPos = Vector3.zero;
            Vector3 firstPosUpper = Vector3.zero;

            for (int i = 0; i <= divided; ++i)
            {
                float t = (float)i / divided;
                splineContainer.Spline.Evaluate( t, out var pos, out var tangent, out _ );

                Vector3 tangentXZ = new Vector3( tangent.x, 0f, tangent.z ).normalized;
                float3 normalXZ = Vector3.Cross( Vector3.up, tangentXZ ).normalized;

                float3 offsetPos = pos + normalXZ * expandFactor;
                //offsetPos.y = uniformY;

                Vector3 posUpper = offsetPos;
                posUpper.y += height;

                if (i == 0)
                {
                    firstOffsetPos = offsetPos;
                    firstPosUpper = posUpper;
                }

                if (splineContainer.Spline.Closed && i == divided)
                {
                    offsetPos = firstOffsetPos;
                    posUpper = firstPosUpper;
                }

                var vertex0 = vertices[2 * i];
                vertex0.Position = offsetPos;
                vertices[2 * i] = vertex0;

                var vertex1 = vertices[2 * i + 1];
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

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }

            // もし reverseFaces が true なら、メッシュの面を反転する
            if (reverseFaces)
            {
                ReverseMeshFaces();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError( $"メッシュの再構築中にエラーが発生しました: {ex.Message}\n{ex.StackTrace}" );
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
        mesh.name = $"{gameObject.name}_Mesh";
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

        // ※reverseFacesフラグで制御しているため、ここでフラグの値を変更しない
        // updating = false;
    }

    private void OnValidate()
    {
        if (meshFilter == null)
        {
            TryGetComponent( out meshFilter );
        }

        if (mesh == null)
        {
            mesh = new Mesh { name = $"{gameObject.name}_Mesh" };
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
