using UnityEngine;

public class PlayerSelectComponent : MonoBehaviour
{
    [SerializeField,Header("�v���C���[�ԍ�")]
    private int playerNumber = 0; // �v���C���[�ԍ�

    [SerializeField,Header("�g�p����L�����N�^�[�f�[�^")]
    private SushiDataScriptableObject playerData;

    [SerializeField, Header("�o���ʒu�̃I�t�Z�b�g")]
    private Vector3 positionOffset;

    [SerializeField, ReadOnly]
    private int selectedCharacterIndex = 0; // �I�����ꂽ�L�����N�^�[�̃C���f�b�N�X


    private void Start()
    {
        // �I�񂾃L�����N�^�[�̃C���f�b�N�X���擾
        selectedCharacterIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex( playerNumber );

        var playerPrefab = playerData.GetItem(selectedCharacterIndex ).sushiPrefub;

        // ���̏ꏊ�Ő���
        var sushiObject = Instantiate( playerPrefab, transform.position + positionOffset, transform.rotation );

        var setUpData = sushiObject.GetComponent<PlayerSetUp>();
        setUpData.SetUpPlayer(playerNumber);
    }
}
