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
    private float expandFactor = 0.5f; // ���b�V�����g�k���銄��

    [SerializeField, Range(200, 1000)]
    private int splinePointReduction = 1000; // �X�v���C���|�C���g�̌��_��

    private float cachedLength = -1f;

    private void Awake()
    {
        // ���b�V���̓Ɨ������m��
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
            Debug.LogError( "SplineContainer �R���|�[�l���g��������܂���B" );
        }

        if ( !TryGetComponent( out meshFilter ) )
        {
            Debug.LogError( "MeshFilter �R���|�[�l���g��������܂���B" );
        }

        if ( mesh == null )
        {
            mesh = new Mesh();

            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }
        else if ( meshFilter.sharedMesh != mesh )
        {
            // ���̃I�u�W�F�N�g�ƃ��b�V�������L���Ă���ꍇ�A�V�������b�V�����쐬
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }

        if ( !TryGetComponent( out meshCollider ) )
        {
            Debug.LogError( "MeshCollider �R���|�[�l���g��������܂���B" );
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
            Debug.LogWarning( "Spline ���ݒ肳��Ă��܂���B" );
            return;
        }

        // �X�v���C���̑������擾�i�L���b�V�����g�p�j
        float totalLength = GetSplineLength(splineContainer.Spline);

        // divided ���v�Z
        int divided = Mathf.Max(2, Mathf.CeilToInt(totalLength / segmentLength));

        mesh.Clear();

        try
        {
            // 'using' �u���b�N���폜
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

                // �ꎞ�ϐ��ɑ�����Ă���ݒ�
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
            Mesh.ApplyAndDisposeWritableMeshData( meshDataArray, mesh ); // ������Dispose����邽�߁A�����ł�Dispose�͕s�v

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            // �R���C�_�[���X�V
            if ( meshCollider != null )
            {
                meshCollider.sharedMesh = null; // �����̃R���C�_�[���N���A
                meshCollider.sharedMesh = mesh; // �V�������b�V����ݒ�
            }
        }
        catch ( Exception ex )
        {
            Debug.LogError( $"���b�V���̍č\�z���ɃG���[���������܂���: {ex.Message}\n{ex.StackTrace}" );
        }
    }



    public void GenerateSplineFromMesh()
    {
        if ( mesh == null )
        {
            Debug.LogError( "���b�V�������݂��܂���B" );
            return;
        }

        // ���b�V���̒��_���������ł��邱�Ƃ��m�F
        if ( mesh.vertexCount % 2 != 0 )
        {
            Debug.LogError( "���b�V���̒��_�����s���ł��B���_���͋����ł���K�v������܂��B" );
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int totalPoints = vertices.Length / 2;

        // �X�v���C���|�C���g�̌��_�����L���͈͓��ł��邱�Ƃ��m�F
        splinePointReduction = Mathf.Clamp( splinePointReduction, 1, totalPoints );

        // ���_���Ɋ�Â��ă|�C���g�����v�Z
        int pointCount = Mathf.CeilToInt((float)totalPoints / splinePointReduction);

        // �����̃X�v���C�����N���A
        splineContainer.Spline.Clear();

        for ( int i = 0; i < totalPoints; i += splinePointReduction )
        {
            Vector3 position = vertices[2 * i];

            // �X�v���C���|�C���g�̒ǉ�
            splineContainer.Spline.Add( new BezierKnot( position, 0f, 0f, Quaternion.identity ) );
        }

        // �Ō�̃|�C���g���X�v���C���Ɋ܂܂�Ă��Ȃ��ꍇ�A�ǉ�
        if ( ( totalPoints - 1 ) % splinePointReduction != 0 )
        {
            Vector3 lastPosition = vertices[2 * (totalPoints - 1)];
            splineContainer.Spline.Add( new BezierKnot( lastPosition, 0f, 0f, Quaternion.identity ) );
        }

        // �X�v���C�����X�V
        //splineContainer.Spline.ComputeTangents();
        //splineContainer.RefreshAll();

        Debug.Log( "���b�V������X�v���C���𐶐����܂����B" );
    }



    /// <summary>
    /// �X�v���C���̑������v�Z���܂��B�L���b�V���𗘗p���ăp�t�H�[�}���X�����コ���܂��B
    /// </summary>
    /// <param name="spline">�Ώۂ̃X�v���C��</param>
    /// <returns>�X�v���C���̑���</returns>
    private float GetSplineLength( Spline spline )
    {
        if ( cachedLength < 0f )
        {
            cachedLength = CalculateSplineLength( spline );
        }
        return cachedLength;
    }

    /// <summary>
    /// �X�v���C���̑������v�Z���܂��B
    /// </summary>
    /// <param name="spline">�Ώۂ̃X�v���C��</param>
    /// <returns>�X�v���C���̑���</returns>
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
    /// ���b�V���̖ʂ𔽑΂ɂ��܂��i�@���𔽓]�j�B
    /// </summary>
    public void ReverseMeshFaces()
    {
        if ( mesh == null )
        {
            Debug.LogError( "���b�V�������݂��܂���B" );
            return;
        }

        // ���b�V���̓Ɨ������m�ۂ��邽�߂ɐV�������b�V�����쐬
        mesh = Instantiate( mesh );
        mesh.name = $"{gameObject.name}_ReversedMesh";
        meshFilter.mesh = mesh;

        var triangles = mesh.triangles;
        for ( int i = 0; i < triangles.Length; i += 3 )
        {
            // ���_�̏������t�ɂ���
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // �R���C�_�[���X�V
        if ( meshCollider != null )
        {
            meshCollider.sharedMesh = null; // �����̃R���C�_�[���N���A
            meshCollider.sharedMesh = mesh; // �V�������b�V����ݒ�
        }

        updating = false; // �X�V�t���O�����Z�b�g
    }

    public void ExpandMeshInnerSide()
    {
        if ( mesh == null )
        {
            Debug.LogError( "���b�V�������݂��܂���B" );
            return;
        }

        // ���b�V���̓Ɨ������m�ۂ��邽�߂ɐV�������b�V�����쐬
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

        // �R���C�_�[���X�V
        if ( meshCollider != null )
        {
            meshCollider.sharedMesh = null; // �����̃R���C�_�[���N���A
            meshCollider.sharedMesh = mesh; // �V�������b�V����ݒ�
        }

        updating = false; // �X�V�t���O�����Z�b�g
    }


    /// <summary>
    /// �G�f�B�^�[��ł̕ύX�����o���A���b�V���̓Ɨ������m�ۂ��܂��B
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
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }

        if ( meshFilter != null && meshFilter.sharedMesh != mesh )
        {
            // ���̃I�u�W�F�N�g�ƃ��b�V�������L���Ă���ꍇ�A�V�������b�V�����쐬
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }
    }
}
