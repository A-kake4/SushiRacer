using UnityEngine;
//-------------------------------------
// �I�u�W�F�N�g����]�����邽�߂̃X�N���v�g
//--------------------------------------
// SpinInput�̉�]�𗘗p���Ė��t���[����]���s��

[RequireComponent( typeof( SushiComponent ) )]
public class SpinObject : MonoBehaviour
{
    [SerializeField]
    private SushiComponent sushiComponent; // �Ώۂ�SushiComponent

    [SerializeField]
    private SpinImput spinInput; // SpinInput�̎Q��

    [SerializeField]
    private Transform spinObject; // ��]������I�u�W�F�N�g

    [SerializeField, Header( "��]��" )]
    private AudioComponent spinSound;

    [SerializeField, ReadOnly]
    private float rotationSpeed = 1f; // ��]���x�̔{��



    private void Start()
    {
        rotationSpeed = sushiComponent.GetSushiData().rotationSpeed;
    }

    private void FixedUpdate()
    {
        var spinSpeed = spinInput.NowSpinSpeed;
        var maxSpinSpeed = spinInput.MaxSpinSpeed;

        // �ő呬�x��1�Ƃ����ꍇ�̉�]���x���v�Z
        float rotationAmount = spinSpeed / (float)maxSpinSpeed;

        // ��]���̍Đ��E��~
        if ( spinSound != null )
        {
            if ( rotationAmount > 0f )
            {
                if ( !spinSound.IsPlaying )
                {
                    spinSound.Play();
                }
                spinSound.SetVolume = rotationAmount;
            }
            else
            {
                if ( spinSound.IsPlaying )
                {
                    spinSound.Stop();
                }
            }
        }

        // ��]�ʂ��v�Z
        float rotationAngle = rotationAmount * rotationSpeed * Time.fixedDeltaTime;
        // ��]�������v�Z
        Vector3 rotationDirection = Vector3.up * rotationAngle;
        // �I�u�W�F�N�g����]
        if ( spinObject != null )
        {
            spinObject.Rotate( rotationDirection, Space.World );
        }
        else
        {
            Debug.LogWarning( "SpinObject: spinObject is not assigned." );
        }
    }
}
