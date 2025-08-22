using UnityEngine;

public class PlayerSetUp : MonoBehaviour
{
    [SerializeField, Header("�v���C���[�̃R���|�[�l���g")]
    private SushiComponent sushiComponent; // �Ώۂ�SushiComponent

    [SerializeField, Header("�v���C���[�̃f�[�^")]
    private PlayerWindow playerWindow; // �v���C���[�̃E�B���h�E�f�[�^

    [SerializeField, Header("����ւ��ɕK�v�ȃR���|�[�l���g�Q")]
    private Camera mainCamera; // ���C���J����
    [SerializeField]
    private Camera effectCamera;
    [SerializeField]
    private GameObject effectLayerObject; // �v���C���[�̃��C���[�}�X�N

    public void SetUpPlayer(int playerNumber)
    {
        sushiComponent.PlayerNumber = playerNumber;

        mainCamera.rect = playerWindow.GetCameraDatas(playerNumber).viewportRect;
        effectCamera.rect = playerWindow.GetCameraDatas( playerNumber ).viewportRect;

        int layerNum = playerWindow.GetCameraDatas( playerNumber ).effectLayer;

        effectCamera.cullingMask = layerNum;
        effectLayerObject.layer = (int)Mathf.Log( layerNum, 2 );

        // �Q�[���p�b�h�̓��͐ݒ�
        var device = PlayerSelectManager.Instance.GetPlayerDevice( playerNumber );
        if ( device == null )
        {
            Debug.LogError( $"�v���C���[{playerNumber}�Ɋ��蓖�Ă�ꂽ�f�o�C�X�� null �ł��B" );
        }
        else
        {
            Debug.Log( $"�v���C���[{playerNumber}�Ɋ��蓖�Ă�ꂽ�f�o�C�X: {device}" );
        }

        // ���̓}�l�[�W���Ƀf�o�C�X��o�^
        InputManager.Instance.RegisterPlayerDevice( playerNumber, device );
    }
}
