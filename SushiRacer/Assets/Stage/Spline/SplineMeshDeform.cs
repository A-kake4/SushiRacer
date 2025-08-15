using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMeshDeform : MonoBehaviour
{
    // �X�v���C�������i�[����R���|�[�l���g
    [SerializeField] private SplineContainer _container;

    // �X�v���C����̈ʒu�������p�����[�^�i0�`1�͈̔͂Ŏw��j
    [SerializeField, Range(0, 1)] private float _t;

    // ���b�V���̑O�����̎�
    [SerializeField] private Vector3 _forwardAxis = Vector3.forward;

    // ���b�V���̏�����̎�
    [SerializeField] private Vector3 _upAxis = Vector3.up;

    private bool _isStarted;

    // �ό`�Ώۂ̃��b�V��
    private MeshFilter _meshFilter;

    // ���b�V���R���C�_�[�i�I�v�V�����j
    private MeshCollider _meshCollider;

    // �ό`���ꂽ���b�V�����
    private Mesh _deformedMesh;
    private Vector3[] _originalVertices;
    private Vector3[] _originalNormals;

    // ���b�V����ό`����
    public void Deform()
    {
        if ( _container == null || _meshFilter == null )
            return;

        if ( _deformedMesh == null )
        {
            // ���b�V�����̃R�s�[
            _deformedMesh = _meshFilter.mesh;
            _originalVertices = _deformedMesh.vertices;
            _originalNormals = _deformedMesh.normals;
        }

        // ��������̉�]�N�H�[�^�j�I�����v�Z
        var axisRotation = Quaternion.Inverse(Quaternion.LookRotation(_forwardAxis, _upAxis));

        // �X�v���C���̃��[�J�����W���烁�b�V���̃��[�J�����W�ɕϊ�����s��
        // �X�v���C���̃��[�J�����W�����[���h���W�����b�V���̃��[�J�����W�̏��ŕϊ�����K�v������
        var splineToLocalMatrix = transform.worldToLocalMatrix * _container.transform.localToWorldMatrix;

        // ���b�V���̒��_�����ꎞ�I�Ɋi�[����z��
        NativeArray<float3> deformedVertices = default;
        NativeArray<float3> deformedNormals = default;

        try
        {
            // �z��̊m��
            deformedVertices = new NativeArray<float3>( _originalVertices.Length, Allocator.Temp );
            deformedNormals = new NativeArray<float3>( _originalNormals.Length, Allocator.Temp );

            // �X�v���C���̏����i�[����NativeSpline���쐬
            using var spline = new NativeSpline(_container.Spline, splineToLocalMatrix);

            // �X�v���C���̒������v�Z
            var splineLength = spline.CalculateLength(splineToLocalMatrix);

            // ���b�V���̒��_�����X�v���C���ɉ����ĕό`
            for ( var i = 0; i < deformedVertices.Length; i++ )
            {
                // ���̒��_���W
                var originalVertex = _originalVertices[i];

                // ���̉�]�␳
                originalVertex = math.mul( axisRotation, originalVertex );

                // ���_���W�̑O�����������X�v���C����̈ʒu�ɕϊ�
                var t = originalVertex.z / splineLength + _t;

                // �͂܂ꂽ�X�v���C���Ȃ�At�̒l�����[�v������
                if ( spline.Closed ) t = Mathf.Repeat( t, 1 );

                // �v�Z���ꂽt�ɒu����ʒu�E�ڐ��E������x�N�g�����擾
                spline.Evaluate(
                    t,
                    out var splinePos,
                    out var splineTangent,
                    out var splineUp
                );

                // �X�v���C���ɑ΂���I�t�Z�b�g�̉�]�N�H�[�^�j�I��
                var rotation = quaternion.LookRotationSafe(splineTangent, splineUp);

                // �X�v���C����̈ʒu�ɑ΂��Đ��������Ȃ��炵�ʒu���v�Z
                var offset = math.mul(rotation, new float3(originalVertex.x, originalVertex.y, 0));

                // �X�v���C���ʒu�ɑ΂��钸�_���W���v�Z
                deformedVertices[i] = splinePos + offset;

                // �X�v���C���ʒu�ɑ΂���@���x�N�g�����v�Z
                // �@���ɂ����̉�]�␳��K�p
                var normal = math.mul(axisRotation, _originalNormals[i]);
                deformedNormals[i] = math.mul( rotation, normal );
            }

            // ���b�V�������X�V
            _deformedMesh.SetVertices( deformedVertices );
            _deformedMesh.SetNormals( deformedNormals );

            // �o�E���f�B���O�{�b�N�X�̍Čv�Z
            _deformedMesh.RecalculateBounds();

            // ���b�V���R���C�_�[�̍X�V
            if ( _meshCollider != null )
                _meshCollider.sharedMesh = _deformedMesh;
        }
        finally
        {
            // ���������
            if ( deformedVertices.IsCreated ) deformedVertices.Dispose();
            if ( deformedNormals.IsCreated ) deformedNormals.Dispose();
        }
    }

    // ����������
    private void Start()
    {
        _isStarted = true;

        TryGetComponent( out _meshFilter );
        TryGetComponent( out _meshCollider );

        // ����ɕό`���s��
        Deform();
    }

    // �㏈��
    private void OnDestroy()
    {
        // MeshFilter����擾�������b�V���͖����I�ɔj������K�v������
        if ( _deformedMesh != null )
            Destroy( _deformedMesh );

        _deformedMesh = null;
    }

#if UNITY_EDITOR
    // �C���X�y�N�^�[���瑀�삳�ꂽ�ۂɕό`�𔽉f
    private void OnValidate()
    {
        // �J�n�O��OnValidate���Ă΂�邱�Ƃ����邽�߁A_isStarted���m�F
        if ( !_isStarted ) return;

        Deform();
    }
#endif
}