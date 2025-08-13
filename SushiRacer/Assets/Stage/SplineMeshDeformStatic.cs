using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class SplineMeshDeformStatic : MonoBehaviour
{
    // �X�v���C�������i�[����R���|�[�l���g
    [SerializeField] private SplineContainer _container;

    // �ό`�Ώۂ̃��b�V��
    [SerializeField] private Mesh _sourceMesh;

    // �ό`��̃��b�V��
    [SerializeField] private Mesh _deformedMesh;

    [SerializeField, Tooltip( "���b�V�����X�v���C����Ń��[�v������" )]
    private bool _loopMesh = false;

    // �X�v���C����̈ʒu�������p�����[�^�i0�`1�͈̔͂Ŏw��j
    [SerializeField, Range( 0, 1 )] private float _t;

    // ���b�V���̑O�����̎�
    [SerializeField] private Vector3 _forwardAxis = Vector3.forward;

    // ���b�V���̏�����̎�
    [SerializeField] private Vector3 _upAxis = Vector3.up;

    private bool _isStarted;

    // ���b�V���R���C�_�[�i�I�v�V�����j
    private MeshCollider _meshCollider;

    // �ό`���ꂽ���b�V�����
    private Vector3[] _originalVertices;
    private Vector3[] _originalNormals;

    // ���b�V����ό`����
    public void Deform()
    {
        if (_container == null || _sourceMesh == null || _deformedMesh == null)
            return;

        _originalVertices ??= _sourceMesh.vertices;
        _originalNormals ??= _sourceMesh.normals;

        var axisRotation = Quaternion.Inverse( Quaternion.LookRotation( _forwardAxis, _upAxis ) );
        var splineToLocalMatrix = transform.worldToLocalMatrix * _container.transform.localToWorldMatrix;

        using var spline = new NativeSpline( _container.Spline, splineToLocalMatrix );
        var splineLength = spline.CalculateLength( splineToLocalMatrix );

        // ���b�V���̒����iZ�������̍ő�l - �ŏ��l�j
        float meshMinZ = float.MaxValue, meshMaxZ = float.MinValue;
        foreach (var v in _originalVertices)
        {
            var localV = math.mul( axisRotation, v );
            meshMinZ = Mathf.Min( meshMinZ, localV.z );
            meshMaxZ = Mathf.Max( meshMaxZ, localV.z );
        }
        float meshLength = meshMaxZ - meshMinZ;

        // ���[�v�����v�Z
        int loopCount = 1;
        if (_loopMesh && meshLength > 0)
            loopCount = Mathf.CeilToInt( splineLength / meshLength );

        // ���_�E�@�����X�g
        var allVertices = new System.Collections.Generic.List<Vector3>( _originalVertices.Length * loopCount );
        var allNormals = new System.Collections.Generic.List<Vector3>( _originalNormals.Length * loopCount );

        for (int l = 0; l < loopCount; l++)
        {
            for (int i = 0; i < _originalVertices.Length; i++)
            {
                var originalVertex = _originalVertices[i];
                originalVertex = math.mul( axisRotation, originalVertex );

                // ���b�V����Z�͈͂����[�v�ŃI�t�Z�b�g
                float meshZ = originalVertex.z - meshMinZ + l * meshLength;
                var t = meshZ / splineLength + _t;

                // ���[�v���� t �� 0�`1 �Ɏ��߂�
                if (_loopMesh)
                    t = Mathf.Repeat( t, 1 );

                spline.Evaluate(
                    t,
                    out var splinePos,
                    out var splineTangent,
                    out var splineUp
                );

                var rotation = quaternion.LookRotationSafe( splineTangent, splineUp );
                var offset = math.mul( rotation, new float3( originalVertex.x, originalVertex.y, 0 ) );
                allVertices.Add( splinePos + offset );

                var normal = math.mul( axisRotation, _originalNormals[i] );
                allNormals.Add( math.mul( rotation, normal ) );
            }
        }


        // ���b�V�������X�V
        _deformedMesh.SetVertices( allVertices );
        _deformedMesh.SetNormals( allNormals );
        _deformedMesh.RecalculateBounds();

        if (_meshCollider != null)
            _meshCollider.sharedMesh = _deformedMesh;
    }
    //public void Deform()
    //{
    //    if (_container == null || _sourceMesh == null || _deformedMesh == null)
    //        return;

    //    // ���b�V�����̃R�s�[
    //    _originalVertices ??= _sourceMesh.vertices;
    //    _originalNormals ??= _sourceMesh.normals;

    //    // ��������̉�]�N�H�[�^�j�I�����v�Z
    //    var axisRotation = Quaternion.Inverse( Quaternion.LookRotation( _forwardAxis, _upAxis ) );

    //    // �X�v���C���̃��[�J�����W���烁�b�V���̃��[�J�����W�ɕϊ�����s��
    //    // �X�v���C���̃��[�J�����W�����[���h���W�����b�V���̃��[�J�����W�̏��ŕϊ�����K�v������
    //    var splineToLocalMatrix = transform.worldToLocalMatrix * _container.transform.localToWorldMatrix;

    //    // ���b�V���̒��_�����ꎞ�I�Ɋi�[����z��
    //    NativeArray<float3> deformedVertices = default;
    //    NativeArray<float3> deformedNormals = default;

    //    try
    //    {
    //        // �z��̊m��
    //        deformedVertices = new NativeArray<float3>( _originalVertices.Length, Allocator.Temp );
    //        deformedNormals = new NativeArray<float3>( _originalNormals.Length, Allocator.Temp );

    //        // �X�v���C���̏����i�[����NativeSpline���쐬
    //        using var spline = new NativeSpline( _container.Spline, splineToLocalMatrix );

    //        // �X�v���C���̒������v�Z
    //        var splineLength = spline.CalculateLength( splineToLocalMatrix );

    //        // ���b�V���̒��_�����X�v���C���ɉ����ĕό`
    //        for (var i = 0; i < deformedVertices.Length; i++)
    //        {
    //            // ���̒��_���W
    //            var originalVertex = _originalVertices[i];

    //            // ���̉�]�␳
    //            originalVertex = math.mul( axisRotation, originalVertex );

    //            // ���_���W�̑O�����������X�v���C����̈ʒu�ɕϊ�
    //            var t = originalVertex.z / splineLength + _t;

    //            // �͂܂ꂽ�X�v���C���Ȃ�At�̒l�����[�v������
    //            if (spline.Closed)
    //                t = Mathf.Repeat( t, 1 );

    //            // �v�Z���ꂽt�ɒu����ʒu�E�ڐ��E������x�N�g�����擾
    //            spline.Evaluate(
    //                t,
    //                out var splinePos,
    //                out var splineTangent,
    //                out var splineUp
    //            );

    //            // �X�v���C���ɑ΂���I�t�Z�b�g�̉�]�N�H�[�^�j�I��
    //            var rotation = quaternion.LookRotationSafe( splineTangent, splineUp );

    //            // �X�v���C����̈ʒu�ɑ΂��Đ��������Ȃ��炵�ʒu���v�Z
    //            var offset = math.mul( rotation, new float3( originalVertex.x, originalVertex.y, 0 ) );

    //            // �X�v���C���ʒu�ɑ΂��钸�_���W���v�Z
    //            deformedVertices[i] = splinePos + offset;

    //            // �X�v���C���ʒu�ɑ΂���@���x�N�g�����v�Z
    //            // �@���ɂ����̉�]�␳��K�p
    //            var normal = math.mul( axisRotation, _originalNormals[i] );
    //            deformedNormals[i] = math.mul( rotation, normal );
    //        }

    //        // ���b�V�������X�V
    //        _deformedMesh.SetVertices( deformedVertices );
    //        _deformedMesh.SetNormals( deformedNormals );

    //        // �o�E���f�B���O�{�b�N�X�̍Čv�Z
    //        _deformedMesh.RecalculateBounds();

    //        // ���b�V���R���C�_�[�̍X�V
    //        if (_meshCollider != null)
    //            _meshCollider.sharedMesh = _deformedMesh;
    //    }
    //    finally
    //    {
    //        // ���������
    //        if (deformedVertices.IsCreated)
    //            deformedVertices.Dispose();
    //        if (deformedNormals.IsCreated)
    //            deformedNormals.Dispose();
    //    }
    //}

    // ����������
    private void Start()
    {
        _isStarted = true;

        TryGetComponent( out _meshCollider );

        // ����ɕό`���s��
        Deform();
    }

#if UNITY_EDITOR
    // �C���X�y�N�^�[���瑀�삳�ꂽ�ۂɕό`�𔽉f
    private void OnValidate()
    {
        if (_sourceMesh == null)
            return;

        if (UnityEditor.EditorApplication.isPlaying)
        {
            // �J�n�O��OnValidate���Ă΂�邱�Ƃ����邽�߁A_isStarted���m�F
            if (_isStarted)
                Deform();
        }
        else
        {
            // �V�������b�V���A�Z�b�g���쐬
            if (_deformedMesh == null)
                _deformedMesh = CreateMeshAsset();

            if (_deformedMesh == null)
                return;

            Deform();
        }
    }

    private void Reset()
    {
        // MeshFilter������΁A���̃��b�V������͂Ƃ���
        if (TryGetComponent<MeshFilter>( out var meshFilter ))
        {
            _sourceMesh = meshFilter.sharedMesh;
            _deformedMesh = CreateMeshAsset();

            // MeshFilter��ݒ�
            meshFilter.sharedMesh = _deformedMesh;
        }

        // MeshCollider������΁A���l�ɐݒ�
        if (TryGetComponent<MeshCollider>( out var meshCollider ))
            meshCollider.sharedMesh = _deformedMesh;
    }

    // ���b�V���A�Z�b�g�̍쐬����
    // ������SplinesExtrude�̂��̂��Q�l�ɂ��܂���
    private Mesh CreateMeshAsset()
    {
        if (_sourceMesh == null)
            return null;

        // �V�������b�V���A�Z�b�g�𕡐�
        var mesh = Instantiate( _sourceMesh );
        mesh.name = name;

        // ���b�V���A�Z�b�g�̕ۑ��p�X������
        var scene = SceneManager.GetActiveScene();
        var sceneDataDir = "Assets";

        if (!string.IsNullOrEmpty( scene.path ))
        {
            var dir = Path.GetDirectoryName( scene.path );
            sceneDataDir = $"{dir}/{Path.GetFileNameWithoutExtension( scene.path )}";
            if (!Directory.Exists( sceneDataDir ))
                Directory.CreateDirectory( sceneDataDir );
        }

        var path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(
            $"{sceneDataDir}/SplineMesh_{mesh.name}.asset" );

        // ���b�V���A�Z�b�g�̕ۑ�
        UnityEditor.AssetDatabase.CreateAsset( mesh, path );

        // ���b�V���A�Z�b�g��I����Ԃɂ���
        UnityEditor.EditorGUIUtility.PingObject( mesh );

        return mesh;
    }
#endif
}