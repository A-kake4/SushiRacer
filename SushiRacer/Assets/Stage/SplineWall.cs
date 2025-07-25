using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Unity.Mathematics; // �ǉ�

[StructLayout( LayoutKind.Sequential )]
struct VertexData
{
    public Vector3 Position;
}

[RequireComponent( typeof( SplineContainer ), typeof( MeshFilter ), typeof( MeshCollider ) )]
public class SplineWall : MonoBehaviour
{
    [SerializeField, Tooltip( "�X�v���C���Ƀ��b�V�������킹�邩" )]
    private bool updating = true;

    // �ǉ�: Y ���W���ψꉻ���邽�߂̖ڕW�l
    [SerializeField, Tooltip( "�e�X�v���C���� Y ���W���ψꉻ����l" )]
    private float uniformY = 0f;

    [SerializeField, Range( 0.1f, 100f ), Tooltip( "�ǂ̒��_�Ԃ̒���" )]
    private float segmentLength = 1.0f;

    [SerializeField, Tooltip( "�ǂ̍���" )]
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
    private float expandFactor = 0.5f; // ���b�V�����g�k���銄��

    [SerializeField, Range( 200, 1000 )]
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
        if (!TryGetComponent( out splineContainer ))
        {
            Debug.LogError( "SplineContainer �R���|�[�l���g��������܂���B" );
        }

        if (!TryGetComponent( out meshFilter ))
        {
            Debug.LogError( "MeshFilter �R���|�[�l���g��������܂���B" );
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }
        else if (meshFilter.sharedMesh != mesh)
        {
            // ���̃I�u�W�F�N�g�ƃ��b�V�������L���Ă���ꍇ�A�V�������b�V�����쐬
            mesh = Instantiate( meshFilter.sharedMesh );
            mesh.name = $"{gameObject.name}_Mesh";
            meshFilter.mesh = mesh; // sharedMesh �ł͂Ȃ� mesh ���g�p
        }

        if (!TryGetComponent( out meshCollider ))
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
        if (!updating)
        {
            return;
        }

        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "Spline ���ݒ肳��Ă��܂���B" );
            return;
        }

        // �X�v���C���̑������擾�i�L���b�V�����g�p�j
        float totalLength = GetSplineLength( splineContainer.Spline );

        // divided ���v�Z
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
                // �X�v���C������ʒu��]�����AY ���W�͖�������XZ�����݂̂��c��
                splineContainer.Spline.Evaluate( t, out var pos, out _, out _ );
                Vector3 position = new Vector3( pos.x, uniformY, pos.z );

                // �����̒��_��ݒ�
                var vertex0 = vertices[2 * i];
                vertex0.Position = position;
                vertices[2 * i] = vertex0;

                // �㑤�̒��_�� uniformY + height �Ƃ���
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

            // �R���C�_�[���X�V
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError( $"���b�V���̍č\�z���ɃG���[���������܂���: {ex.Message}\n{ex.StackTrace}" );
            return;
        }

        // expandFactor �ɂ������g�k��K�p
        ApplyExpandFactor();
    }

    private void ApplyExpandFactor()
    {
        if (mesh == null)
        {
            Debug.LogError( "���b�V�������݂��܂���B" );
            return;
        }

        // ���b�V���̓Ɨ������m�ۂ��邽�߂ɐV�������b�V���𐶐�
        Mesh expandedMesh = Instantiate( mesh );
        expandedMesh.name = $"{gameObject.name}_ExpandedMesh";
        meshFilter.mesh = expandedMesh;

        Vector3[] vertices = expandedMesh.vertices;
        Vector3[] normals = expandedMesh.normals;

        for (int i = 0; i < vertices.Length; i++)
        {
            // �@������ Y �������������AXZ���ʏ�̕����𓾂�
            Vector3 n = normals[i];
            n.y = 0f;
            if (n.sqrMagnitude > 0.0001f)
            {
                n.Normalize();
            }
            // XZ ���ʂ����Ŋg�k��K�p
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
    /// �X�v���C�����̊e Knot �� Y ���W�� uniformY �̒l�ɋψꉻ���܂��B
    /// </summary>
    public void UniformizeSplineY()
    {
        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "�X�v���C�����ݒ肳��Ă��܂���B" );
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
        Debug.Log( $"�X�v���C����Y���W�� {uniformY} �ɋψꉻ����܂����B" );
    }

    /// <summary>
    /// �X�v���C�����̊e Knot �̉�]�� X, Z �����Z�b�g���AY ���̒l�̂ݎc���܂��B
    /// </summary>
    public void ResetSplineRotation()
    {
        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "�X�v���C�����ݒ肳��Ă��܂���B" );
            return;
        }

        int knotCount = splineContainer.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            // BezierKnot �^�ł���Ɖ���
            BezierKnot knot = (BezierKnot)splineContainer.Spline[i];

            // math.quaternion �� UnityEngine.Quaternion �ɕϊ����� Euler �p���擾
            UnityEngine.Quaternion unityQuat = new UnityEngine.Quaternion( knot.Rotation.value.x, knot.Rotation.value.y, knot.Rotation.value.z, knot.Rotation.value.w );
            float yRotationDegrees = unityQuat.eulerAngles.y;
            // EulerXYZ() �̓��W�A����z�肵�Ă��邽�ߕϊ�
            float yRotationRadians = math.radians( yRotationDegrees );

            // X, Z �� 0 �ɂ��� Y ���̂ݎc���V���ȉ�]��ݒ�
            knot.Rotation = quaternion.EulerXYZ( new float3( 0f, yRotationRadians, 0f ) );
            splineContainer.Spline[i] = knot;
        }
        Debug.Log( "�X�v���C���̉�]��Y�������c���`�Ń��Z�b�g����܂����B" );
    }


    public void GenerateSplineFromMesh()
    {
        if (mesh == null)
        {
            Debug.LogError( "���b�V�������݂��܂���B" );
            return;
        }

        if (mesh.vertexCount % 2 != 0)
        {
            Debug.LogError( "���b�V���̒��_�����s���ł��B���_���͋����ł���K�v������܂��B" );
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

        Debug.Log( "���b�V������X�v���C���𐶐����܂����B" );
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
            Debug.LogError( "���b�V�������݂��܂���B" );
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
