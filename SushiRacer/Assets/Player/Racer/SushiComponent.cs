using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BaseComponent<SushiItem, SushiDataScriptableObject> ), true )]
public class SushiComponentEditor : BaseComponentEditor{}
#endif

public enum SushiMode
{
    Event = 0, // �C�x���g���[�h
    Normal = 1, // �ʏ탂�[�h
    DriftWall = 2 // �h���t�g�E�H�[�����[�h
}

public class SushiComponent : BaseComponent<SushiItem, SushiDataScriptableObject>
{
    [SerializeField,Header("�v���C���[�̔ԍ�")]
    private int playerNumber = 0; // �v���C���[�̔ԍ�

    public int PlayerNumber
    {
        set => playerNumber = value;
        get => playerNumber;
    }

    [SerializeField]
    private SushiMode sushiMode = SushiMode.Event;

    [SerializeField, Header( "�v���C���[��Ǐ]����J����" )]
    private PlayerFocusCamera3d playerFocusCamera; // �v���C���[��Ǐ]����J����

    [SerializeField, ReadOnly]
    private Rigidbody rb;
    public Rigidbody Rigidbody
    {
        get => rb;
    }

    [SerializeField, ReadOnly]
    private SpinImput spinImput; // SpinImput�X�N���v�g�̎Q��

    [SerializeField, Header( "�h���t�g�E�H�[���̈ړ��R���|�[�l���g" )]
    private SplineAnimateRigidbody splineAnimateRigidbody; // �X�v���C���A�j���[�V�����̎Q��
    public SplineAnimateRigidbody SplineAnimateRigidbody
    {
        get => splineAnimateRigidbody;
    }

    [SerializeField, ReadOnly]
    private PlayerTotalVelocity3d playerTotalVelocity; // �v���C���[�̑������x�X�N���v�g



    [SerializeField, ReadOnly]
    private float accelSideRate = 10f; // �v���C���[�̉����x
    [SerializeField, ReadOnly]
    private float maxSideSpeed = 20f; // �v���C���[�̍ő呬�x

    [SerializeField, ReadOnly]
    private float rotationMinSpeed = 0.1f;
    [SerializeField, ReadOnly]
    private float rotationMaxSpeed = 3f; // �v���C���[�̉�]���x
    [SerializeField, ReadOnly]
    private float gierRatio = 1f; // �v���C���[�̃M�A��

    private Vector3 moveDirection; // �v���C���[�̈ړ�����

    private bool started = false;

    public SushiItem GetSushiData()
    {
        return dataSource.GetItem(selectedItemNumber);
    }

    private void Start()
    {
        var sushiData = dataSource.GetItem( selectedItemNumber );
        accelSideRate = sushiData.accelSideRate;
        maxSideSpeed = sushiData.maxSideSpeed;
        rotationMaxSpeed = sushiData.rotationMaxSpeed;
        rotationMinSpeed = sushiData.rotationMinSpeed;
        gierRatio = sushiData.gierRatio;
    }

    private void Reset()
    {
        // �R���|�[�l���g�̎Q�Ǝ擾
        playerTotalVelocity = GetComponent<PlayerTotalVelocity3d>();
        spinImput = GetComponent<SpinImput>();
        rb = GetComponent<Rigidbody>();
    }

    public SushiMode GetSushiMode()
    {
        return sushiMode;
    }

    public void SetSushiMode( SushiMode mode )
    {
        sushiMode = mode;
        switch (mode)
        {
            case SushiMode.Event: // �C�x���g���[�h

                // �C�x���g���[�h�ł͉�]���Œ�
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true; // ���I�ɂ��Ȃ�

                playerTotalVelocity.SetLinearVelocity( Vector3.zero );

                playerFocusCamera.FocusMode = 0; // �J�����̃t�H�[�J�X���[�h���C�x���g�ɐݒ�

                break;
            case SushiMode.Normal: // �ʏ탂�[�h

                // �ʏ탂�[�h�ł͉�]���Œ�
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                                | RigidbodyConstraints.FreezeRotationY
                                | RigidbodyConstraints.FreezeRotationZ;

                rb.useGravity = true; // �d�͂�L���ɂ���

                rb.isKinematic = false; // ���I�ɂ���
                playerFocusCamera.FocusMode = 1; // �J�����̃t�H�[�J�X���[�h��ʏ�ɐݒ�
                break;
            case SushiMode.DriftWall: // �h���t�g�E�H�[�����[�h

                // �h���t�g�E�H�[�����[�h�ł͉�]���Œ�
                rb.constraints = RigidbodyConstraints.FreezeRotationX
                                | RigidbodyConstraints.FreezeRotationY
                                | RigidbodyConstraints.FreezeRotationZ;

                rb.useGravity = false; // �d�͂𖳌��ɂ���

                rb.isKinematic = false; // ���I�ɂ���

                playerTotalVelocity.SetLinearVelocity( Vector3.zero ); // ���������x�����Z�b�g
                playerFocusCamera.FocusMode = 2; // �J�����̃t�H�[�J�X���[�h���h���t�g�E�H�[���ɐݒ�

                break;
            default:
                Debug.LogWarning( "Invalid sushi mode selected." );
                break;
        }
    }

    private void FixedUpdate()
    {
        if ( sushiMode == SushiMode.Event )
        {
            if ( CountDownAnimation_Tsuji.instance.GetIsStarted() )
            {
                if ( !started )
                {
                    SetSushiMode( SushiMode.Normal );

                    started = true;
                }
            }

            return;
        }
        if ( sushiMode == SushiMode.Normal )
        {
            PlayerMove();
            return;
        }
        if (sushiMode == SushiMode.DriftWall)
        {
            splineAnimateRigidbody.SpeedFactor = spinImput.NowSpinSpeed * 0.02f * gierRatio; // �X�s�����x�ɉ����Ĉړ����x�𒲐�

            var moveInput = spinImput.MoveInput;
            if (moveInput.sqrMagnitude < 0.01f)
            {
                // ���͂��قƂ�ǂȂ��ꍇ�͈ړ����Ȃ�
            }
            else
            {
                float nowSpinSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;
                int maxSpinSpeed = spinImput.MaxSpinSpeed;

                float rotationSpeed = Mathf.Lerp(
                    rotationMaxSpeed * Time.fixedDeltaTime,
                    rotationMinSpeed * Time.fixedDeltaTime,
                    nowSpinSpeed / maxSpinSpeed );

                splineAnimateRigidbody.OffsetPsitionY += rotationSpeed * moveInput.x * 5f; // ���͂ɉ����ăJ������Y����]�𒲐�
            }
            return;
        }
    }

    private void PlayerMove()
    {
        FrontMoveCameraTarget();
        SideMoveCameraTarget();
    }

    private void FrontMoveCameraTarget()
    {
        // SpinImput���猻�݂̉�]���x���擾
        int spinSpeed = spinImput.NowSpinSpeed;
        int maxSpinSpeed = spinImput.MaxSpinSpeed;

        if (spinSpeed < 0)
        {
            spinSpeed *= -1;
        }

        // �ő��]���x��1�Ƃ����ꍇ�̑O���ړ��ʂ��v�Z
        float moveAmount = (float)spinSpeed / maxSpinSpeed;

        float addSpeed = maxSpinSpeed * moveAmount;

        float forwardSpeed = Vector3.Dot( rb.linearVelocity, transform.forward );

        if (forwardSpeed < maxSpinSpeed)
        {
            playerTotalVelocity.AddLinearVelocity( addSpeed * Time.fixedDeltaTime * transform.forward );
        }
    }

    private void SideMoveCameraTarget()
    {
        // �v���C���[�̉E�������擾�i�J�����̕�������v���C���[�̕����֕ύX�j
        Vector3 playerRight = transform.right;

        playerRight.y = 0; // y�������������Đ����ʂł̉E�������擾
        playerRight.Normalize(); // ���K��
        if (playerRight.sqrMagnitude < 0.01f)
        {
            Debug.LogWarning( "Player right direction is too small, cannot move." );
            return; // �v���C���[�̉E����������������ꍇ�͏����𒆎~
        }

        // ���͂��擾�i�ړ��̂݁j
        float horizontalInput =spinImput.MoveInput.x;

        // �ړ��������v�Z
        moveDirection = playerRight * horizontalInput;

        // �v���C���[�̍��E���x���X�V
        Vector3 targetVelocity = moveDirection * maxSideSpeed;
        // ���݂̑��x���擾
        Vector3 currentVelocity = rb.linearVelocity;
        currentVelocity.y = 0; // Y�������������Đ����ʂł̑��x���擾

        // �ڕW���x�Ɍ����ĉ����܂��͌���
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 accelerationVector = moveDirection.normalized * accelSideRate;

            //playerTotalVelocity.AddAngularVelocity(
            //    Vector3.MoveTowards( currentVelocity, targetVelocity, accelerationVector.magnitude ) );

            // ��]�̍ő呬�x��1�Ƃ��čő��]���s��
            float nowSpinSpeed = spinImput.NowSpinSpeed < 0 ? -spinImput.NowSpinSpeed : spinImput.NowSpinSpeed;
            int maxSpinSpeed = spinImput.MaxSpinSpeed;

            float rotationSpeed = Mathf.Lerp(
                rotationMaxSpeed * Time.fixedDeltaTime,
                rotationMinSpeed * Time.fixedDeltaTime,
                nowSpinSpeed / maxSpinSpeed );

            // �v���C���[�̌������ړ������ɍ��킹�ĉ�]
            Quaternion targetRotation = Quaternion.LookRotation( moveDirection, Vector3.up );
            rb.rotation = Quaternion.Slerp( rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime );
        }
    }
}