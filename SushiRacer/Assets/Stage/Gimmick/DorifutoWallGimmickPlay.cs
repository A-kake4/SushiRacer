using UnityEngine;
using UnityEngine.Splines;

public class DorifutoWallGimmickPlay : BaseGimmickPlayCompornent
{
    [SerializeField, Header( "�h���t�g�E�H�[���̈ړ��R���|�[�l���g" )]
    private SplineContainer splineContainer = default;
    public SplineContainer SplineContainer
    {
        get { return splineContainer; }
    }

    [SerializeField, Header( "�h���t�g�E�H�[���̊�{��" )]
    private float basicWidth = 0.5f; // �h���t�g�E�H�[���̊�{���i�K�v�ɉ����Ē����j
    public float BasicWidth
    {
        get { return basicWidth; }
    }

    private int hitRight = 1; // �E���̐ڐG
    public int HitRight
    {
        get { return hitRight; }
    }

    private void Reset()
    {
        // �h���t�g�E�H�[���̈ړ��R���|�[�l���g�̏�����
        splineContainer = GetComponent<SplineContainer>();
    }

    public override void GimmickPlayTriggerEnter( BaseGimmickMoveCompornent hitObject )
    {
        // �ڐG�ʒu�Ƃ���hitObject�̈ʒu�𗘗p�i�K�v�ɉ����ďC���j
        Vector3 contactPoint = hitObject.transform.position;
        hitRight = DetermineContactSide( splineContainer, contactPoint );

        hitObject.GimmickMoveDorifutoWall( this );
    }

    // �X�v���C����ŐڐG�_�ɍł��߂��p�����[�^�����߂܂��B
    private float FindClosestParameter( Spline spline, Vector3 point )
    {
        float closestParam = 0f;
        float minDist = float.MaxValue;
        const int samples = 100;
        for (int i = 0; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 samplePos = spline.EvaluatePosition( t );
            float dist = Vector3.Distance( point, samplePos );
            if (dist < minDist)
            {
                minDist = dist;
                closestParam = t;
            }
        }
        return closestParam;
    }

    // �ڐG�_���X�v���C����̂ǂ��瑤���𔻒f���܂��B
    // �߂�l: "Right", "Left", "Center", "Unknown"
    private int DetermineContactSide( SplineContainer splineContainer, Vector3 contactPoint )
    {
        if (splineContainer == null || splineContainer.Spline == null)
            return 0;

        float t = FindClosestParameter( splineContainer.Spline, contactPoint );
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition( t );
        // float3��Vector3�ɕϊ����Ă��琳�K��
        Vector3 tangent = ( (Vector3)splineContainer.Spline.EvaluateTangent( t ) ).normalized;
        Vector3 diff = ( contactPoint - splinePos ).normalized;

        // �ڐ��ƐڐG�����̊O�ς̏���������ō��E����
        float crossY = Vector3.Dot( Vector3.Cross( tangent, diff ), Vector3.up );
        if (crossY > 0)
            return 1;
        else
            return -1;
    }
}