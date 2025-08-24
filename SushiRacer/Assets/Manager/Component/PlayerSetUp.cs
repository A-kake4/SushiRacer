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

        if(playerNumber == 0)
        {
            PlayerKeeper_Tsuji.instance.SetPlayer1( gameObject );
        }
        else if(playerNumber == 1)
        {
            PlayerKeeper_Tsuji.instance.SetPlayer2( gameObject );
        }
    }
}
