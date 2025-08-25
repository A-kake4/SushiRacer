using UnityEngine;

[CreateAssetMenu( fileName = "New Sushi Data", menuName = "Sushi/SushiData" )]
public class SushiDataScriptableObject : BaseDataScriptableObject<SushiItem>
{

}

[System.Serializable]
public class SushiSelectData
{
    [Header( "�v���r���[�I�u�W�F�N�g" ), Tooltip( "���[�X�p�̃X�N���v�g���t���Ă��Ȃ����" )]
    public GameObject previewObject;

    [Header( "�X�V�̃A�C�R��" ), Tooltip( "�I����ʂŎg��" )]
    public Sprite sushiIcon;
    [Header( "����" ), Tooltip( "�����A���l�ł͂Ȃ��u�������v�Ƃ��u�ӂ��v�Ƃ�����" )]
    public string speedText;
    [Header( "����" ), Tooltip( "�����A���l�ł͂Ȃ��u�������v�Ƃ��u�ӂ��v�Ƃ�����" )]
    public string accelText;
    [Header( "���쐫" ), Tooltip( "�����A���l�ł͂Ȃ��u�������v�Ƃ��u�ӂ��v�Ƃ�����" )]
    public string rotationText;
    [Header( "������ ( ���s�ł��� )" ), Tooltip( "���s�ł��钷�������A�M�A���u���[�L�������" ), Multiline(6)]
    public string descriptionText;
}

[System.Serializable]
public class SushiItem : BaseItem
{
    [Header( "�X�V�̃v���n�u" ), Tooltip( "�v���C���[���g�p����I�u�W�F�N�g" )]
    public GameObject sushiPrefub;
    [Header( "�O���ړ��@�ő呬�x" ), Tooltip( "�O�����ɐi�ޒʏ펞�̍ő呬�x" )]
    public int maxFrontSpeed = 5000;
    [Header( "�O���ړ��@�����x" ), Tooltip( "�O�����ɐi�ށE�I�u�W�F�N�g�̉�]���Ɏg�������x" )]
    public int accelSpinRate = 20;
    [Header( "�O���ړ��@�����l" ), Tooltip( "�񂵂Ă��Ȃ����̌���" )]
    public int decaySpinRate = 1;
    [Header( "�X�V��]�@�ő呬�x" ), Tooltip( "�I�u�W�F�N�g����鑬���B��������Ƌt��]���Ă�悤�Ɍ�����" )]
    public float rotationSpeed = 3800f; // ��]���x�̔{��
    [Header( "���͂�臒l" ), Tooltip( "�X�e�B�b�N�̃f�b�h�]�[��" )]
    public float inputLimit = 0.1f;
    [Header( "�p�x������臒l" ), Tooltip( "�X�e�B�b�N�̃f�b�h�]�[��" )]
    public float angleLimit = 5f;

    [Header( "���ړ��@�ő呬�x" ), Tooltip( "�����͂��������̉�����B�ő呬�x" )]
    public float maxSideSpeed = 50f;            // �v���C���[�̍ő呬�x
    [Header( "���ړ��@�����x" ), Tooltip( "�����͂��������̉�����B�����x" )]
    public float accelSideRate = 5f;        // �v���C���[�̉����x
    [Header( "�J�������񑬓x�@�ő呬�x��" ), Tooltip( "�X�V���ő��]�̐���\��" )]
    public float rotationMinSpeed = 10f;
    [Header( "�J�������񑬓x�@��~��" ), Tooltip( "�X�V����~���̎��̐���\��" )]
    public float rotationMaxSpeed = 60f;     // �v���C���[�̉�]���x

    [Header( "�u���[�L���x" ), Tooltip( "�X�V���u���[�L�������鑬�x�B0�ŋ}��~" ), Range(0.1f, 1f)]
    public float brakeSpeed = 0.5f; // �u���[�L���x

    [Header( "�M�A��" ), Tooltip( "�ǃh���t�g���̑��x�B1�Œʏ펞�Ɠ����A2��2�{�A0.5�Ŕ����̑��x" )]
    public float gierRatio = 1f; // �M�A��B1�Œʏ�A2��2�{�A0.5�Ŕ����̑��x

    [Header( "�I���f�[�^" ), Tooltip( "�I����ʂŎg���f�[�^" )]
    public SushiSelectData selectData;

    [Header( "�~�j�}�b�v�A�C�R��" )]
    public Material minimapMaterial;
}

