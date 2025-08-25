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
    [SerializeField, Tooltip( "�X�v���C���Ƀ��b�V�������킹�邩" )]
    private bool updating = true;

    [SerializeField, Range( 0.1f, 100f ), Tooltip( "�ǂ̒��_�Ԃ̒���" )]
    private float segmentLength = 1.0f;

    [SerializeField, Tooltip( "�ǂ̍���" )]
    private float height = 5.0f;

    [SerializeField, Range( 0.1f, 10f ), Tooltip( "�ǂ̌���" )]
    private float thickness = 1.0f;

    [SerializeField, ReadOnly]
    private SplineContainer splineContainer;
    [SerializeField, ReadOnly]
    private Mesh mesh;
    [SerializeField, ReadOnly]
    private MeshFilter meshFilter;
    [SerializeField, ReadOnly]
    private MeshCollider meshCollider;

    //[SerializeField, Tooltip( "���b�V���̖ʂ𔽓]���邩�ǂ���" )]
    private bool reverseFaces = false;

    private float cachedLength = -1f;

    public SplineContainer SplineContainerField => splineContainer;

    private void Awake()
    {
        // ���b�V���̓Ɨ����̂��߁Aupdating �� false �ɐݒ�
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
            return;

        if (splineContainer?.Spline == null)
        {
            Debug.LogWarning( "Spline ���ݒ肳��Ă��܂���B" );
            return;
        }

        // ����V�K�� Mesh ���쐬�i���L��Ԃ�h�~�j
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

            // �e�T���v���_�ɂ�4���_�i�f�ʎl�p�`�j�𐶐�
            int vertexCount = 4 * ( divided + 1 );
            // �e�Z�O�����g�ɂ����āA�f�ʂ̊e�ӂ���N���b�h�i2�O�p�`�A�v6���_�j�𐶐�
            int sideIndices = 4 * divided * 6;
            // �X�v���C�������Ă��Ȃ���΃G���h�L���b�v��ǉ��i�J�n�ƏI�����ꂼ��6���_�j
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

            // �e�T���v���_�ɑ΂��Ēf�ʁibottom front, top front, top back, bottom back�j�̒��_�𐶐�
            for (int i = 0; i <= divided; i++)
            {
                float t = (float)i / divided;
                splineContainer.Spline.Evaluate( t, out var pos, out var tangent, out _ );

                // �^���W�F���g�̐��������Ŗ@���v�Z
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
                    // �����X�v���C���̏ꍇ�A�ŏI�f�ʂ��J�n�f�ʂƈ�v������
                    int lastBase = i * 4;
                    vertexArray[lastBase + 0].Position = firstProfile[0];
                    vertexArray[lastBase + 1].Position = firstProfile[1];
                    vertexArray[lastBase + 2].Position = firstProfile[2];
                    vertexArray[lastBase + 3].Position = firstProfile[3];
                }
            }

            // ���_�̏�������
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = vertexArray[i];
            }

            int indexOffset = 0;
            // �e�Z�O�����g�Ԃ̒f�ʂ�A�����đ��ʂ𐶐�
            // �f�ʓ��̒��_�̏���: 0: bottom front, 1: top front, 2: top back, 3: bottom back.
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

            // �X�v���C�������Ă��Ȃ��ꍇ�A�J�n�ƏI���ɃL���b�v��ǉ�
            if (!closed)
            {
                // �J�n�L���b�v�i�ŏ��̒f�ʁj�̃L���b�v
                indices[indexOffset++] = 0;
                indices[indexOffset++] = 2;
                indices[indexOffset++] = 1;

                indices[indexOffset++] = 0;
                indices[indexOffset++] = 3;
                indices[indexOffset++] = 2;

                // �I���L���b�v�i�Ō�̒f�ʁj�̃L���b�v
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
            Debug.LogError( $"���b�V���č\�z���ɃG���[: {ex.Message}\n{ex.StackTrace}" );
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
            Debug.LogError( "���b�V�������݂��܂���B" );
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
            // �G�f�B�^�[���s���̏ꍇ�A�x���Ăяo���Ŏ��s
            EditorApplication.delayCall += () =>
            {
                // �I�u�W�F�N�g�����ɔj������Ă��Ȃ����m�F
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
        // �G�f�B�^�[���s���i�v���C���[�h�łȂ��ꍇ�j�� sharedMesh ��p����
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
        // �v���C���̏ꍇ�͒ʏ�ǂ��� mesh �v���p�e�B��p����
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