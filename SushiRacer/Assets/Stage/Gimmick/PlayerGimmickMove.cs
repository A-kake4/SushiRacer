using UnityEngine;
using UnityEngine.Splines;

public class PlayerGimmickMove : BaseGimmickMoveCompornent
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField] 
    private SushiComponent sushiComponent = default;

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
        float distance = ( 1.3f + gimmickDat.BasicWidth / 2 ) * gimmickDat.HitRight; // �h���t�g�E�H�[���̐ڐG�ʒu����̋������v�Z

        if (sushiComponent.SplineAnimateRigidbody.PlayFromClosestPoint( gimmickDat.SplineContainer, rb, distance ))
        {
            sushiComponent.SetSushiMode( SushiMode.DriftWall ); // �h���t�g�E�H�[�����[�h�ɐݒ�
            return;
        }
    }
}
