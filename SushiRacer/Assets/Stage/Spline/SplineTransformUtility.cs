using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent( typeof( SplineContainer ))]
public class SplineTransformUtility : MonoBehaviour
{
    public float uniformY = 0f; // �X�v���C����Y���W���ψꉻ���邽�߂̒l

    /// <summary>
    /// �w�肳�ꂽ�X�v���C���R���e�i���̊e Knot �̉�]��X, Z�����Z�b�g���AY���̂ݎc���悤�ɒ������܂��B
    /// </summary>
    public void ResetSplineRotation()
    {
        if (!TryGetValidSplineContainer( out SplineContainer container ))
            return;

        int knotCount = container.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            BezierKnot knot = (BezierKnot)container.Spline[i];

            // ���݂�Rotation��Quaternion�ɕϊ����AY���̒l�݂̂��擾
            Quaternion unityQuat = new Quaternion(
                knot.Rotation.value.x,
                knot.Rotation.value.y,
                knot.Rotation.value.z,
                knot.Rotation.value.w );
            float yRotationDegrees = unityQuat.eulerAngles.y;
            float yRotationRadians = math.radians( yRotationDegrees );

            // Y���݂̂𔽉f�����]�ɕύX
            knot.Rotation = quaternion.EulerXYZ( new float3( 0f, yRotationRadians, 0f ) );
            container.Spline[i] = knot;
        }

        Debug.Log( "�X�v���C���̉�]��Y���݂̂�ێ�����`�Ń��Z�b�g����܂����B" );
    }

    /// <summary>
    /// �w�肳�ꂽ�X�v���C���R���e�i���̊e Knot ��Y���W�� uniformY �̒l�ɓ��ꂵ�܂��B
    /// </summary>
    public void UniformizeSplineY()
    {
        if (!TryGetValidSplineContainer( out SplineContainer container ))
            return;

        int knotCount = container.Spline.Count;
        for (int i = 0; i < knotCount; i++)
        {
            BezierKnot knot = (BezierKnot)container.Spline[i];
            Vector3 pos = knot.Position;
            pos.y = uniformY;
            knot.Position = pos;
            container.Spline[i] = knot;
        }
        Debug.Log( $"�X�v���C����Y���W�� {uniformY} �ɋψꉻ����܂����B" );
    }

    /// <summary>
    /// SplineContainer �ƗL���ȃX�v���C�������݂��邩���`�F�b�N���܂��B
    /// </summary>
    /// <param name="container">�擾���ꂽSplineContainer</param>
    /// <returns>�L���ȏꍇ��true�A�����łȂ����false</returns>
    private bool TryGetValidSplineContainer( out SplineContainer container )
    {
        container = GetComponent<SplineContainer>();
        if (container == null)
        {
            Debug.LogWarning( "SplineContainer���A�^�b�`����Ă��܂���B" );
            return false;
        }

        if (container.Spline == null)
        {
            Debug.LogWarning( "�X�v���C�����ݒ肳��Ă��܂���B" );
            return false;
        }
        return true;
    }
}
