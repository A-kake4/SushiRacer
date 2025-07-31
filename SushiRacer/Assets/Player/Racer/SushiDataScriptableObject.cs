using UnityEngine;

[CreateAssetMenu( fileName = "New Sushi Data", menuName = "Sushi/SushiData" )]
public class SushiDataScriptableObject : BaseDataScriptableObject<SushiItem>
{

}

[System.Serializable]
public class SushiItem : BaseItem
{
    [Tooltip( "�X�V�̃v���n�u" )]
    public GameObject sushiPrefub;
    [Tooltip( "�O���ړ��@�ő呬�x" )]
    public int maxFrontSpeed = 5000;
    [Tooltip( "�O���ړ��@�����x" )]
    public int accelSpinRate = 20;
    [Tooltip( "�O���ړ��@�����l" )]
    public int decaySpinRate = 1;
    [Tooltip( "�X�V��]�@�ő呬�x" )]
    public float rotationSpeed = 3800f; // ��]���x�̔{��
    [Tooltip( "���͂�臒l" )]
    public float inputLimit = 0.1f;
    [Tooltip( "�p�x������臒l" )]
    public float angleLimit = 5f;

    [Tooltip( "���ړ��@�ő呬�x" )]
    public float maxSideSpeed = 50f;            // �v���C���[�̍ő呬�x
    [Tooltip( "���ړ��@�����x" )]
    public float accelSideRate = 5f;        // �v���C���[�̉����x
    [Tooltip( "�J�������񑬓x�@�ő呬�x��" )]
    public float rotationMinSpeed = 10f;
    [Tooltip( "�J�������񑬓x�@��~��" )]
    public float rotationMaxSpeed = 60f;     // �v���C���[�̉�]���x
}

