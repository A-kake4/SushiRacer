using UnityEngine;
using UnityEngine.Splines;

public class PlayerGimmickMove : BaseGimmickMoveCompornent
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField] 
    private SushiComponent sushiComponent = default;

    //[SerializeField]
    //private SpinImput spinImput = default;

    [SerializeField, Header("�h���t�g�E�H�[���̈ړ��R���|�[�l���g")]
    private SplineAnimateRigidbody splineAnimateRigidbody = default;


    private void Reset()
    {
        // Rigidbody�̏�����
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

    }

    public override void GimmickMoveDorifutoWall( DorifutoWallGimmickPlay gimmickDat )
    {
        if (splineAnimateRigidbody == null)
        {
            return;
        }
        sushiComponent.SetSushiMode( SushiMode.DriftWall ); // �h���t�g�E�H�[�����[�h�ɐݒ�
        sushiComponent.SplineAnimateRigidbody = splineAnimateRigidbody; // �X�v���C���A�j���[�V�����̐ݒ�

        float distance = (1.3f + gimmickDat.BasicWidth / 2 ) * gimmickDat.HitRight; // �h���t�g�E�H�[���̐ڐG�ʒu����̋������v�Z

        splineAnimateRigidbody.PlayFromClosestPoint( gimmickDat.SplineContainer, rb, distance );
    }
}
