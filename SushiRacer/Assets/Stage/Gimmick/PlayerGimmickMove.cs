using UnityEngine;
using UnityEngine.Splines;

public class PlayerGimmickMove : BaseGimmickMoveCompornent
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField] 
    private SushiComponent sushiComponent = default;

    [SerializeField, Header("�v���C���[�̑傫��")]
    private float playerSize = 1.3f; // �v���C���[�̑傫��

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
        float distance = ( playerSize + gimmickDat.BasicWidth / 2 ) * gimmickDat.HitRight; // �h���t�g�E�H�[���̐ڐG�ʒu����̋������v�Z

        if (sushiComponent.SplineAnimateRigidbody.PlayFromClosestPoint( gimmickDat.SplineContainer, rb, distance ))
        {
            sushiComponent.SetSushiMode( SushiMode.DriftWall ); // �h���t�g�E�H�[�����[�h�ɐݒ�
            return;
        }
    }
}
