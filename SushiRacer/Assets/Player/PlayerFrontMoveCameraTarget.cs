using UnityEngine;
//---------------------------
// �v���C���[�̑O���Ɉړ�������N���X
//------------------------------
// �ړ��ʂ�SpinImput�̉�]�͂𗘗p����
// �ő��]�͂�1�Ƃ����ꍇ�́A
// �v���C���[�̑O���ւ̈ړ��ʂ��v�Z���܂��B

[RequireComponent( typeof( PlayerTotalVelocity3d ) )]
public class PlayerFrontMoveCameraTarget : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private SpinImput spinImput; // SpinImput�X�N���v�g�̎Q��

    [SerializeField, ReadOnly]
    private PlayerTotalVelocity3d playerTotalVelocity; // �v���C���[�̑������x�X�N���v�g
    [SerializeField, ReadOnly]
    private Rigidbody playerRigidbody;

    private void Reset()
    {
        spinImput = GetComponent<SpinImput>();
        playerTotalVelocity = GetComponent<PlayerTotalVelocity3d>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // SpinImput���猻�݂̉�]���x���擾
        int spinSpeed = spinImput.NowSpinSpeed;
        int maxSpinSpeed = spinImput.MaxSpinSpeed;

        if ( spinSpeed < 0 )
        {
            spinSpeed *= -1;
        }


        // �ő��]���x��1�Ƃ����ꍇ�̑O���ړ��ʂ��v�Z
        float moveAmount = (float)spinSpeed / maxSpinSpeed;

        float addSpeed = maxSpinSpeed * moveAmount;

        float forwardSpeed = Vector3.Dot( playerRigidbody.linearVelocity, transform.forward );

        if( forwardSpeed < maxSpinSpeed )
        {
            playerTotalVelocity.AddLinearVelocity( addSpeed * Time.fixedDeltaTime * transform.forward );
        }
    }
}
