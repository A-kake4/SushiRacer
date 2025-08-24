using UnityEngine;

public class PlayerSetUp : MonoBehaviour
{
    [SerializeField, Header("プレイヤーのコンポーネント")]
    private SushiComponent sushiComponent; // 対象のSushiComponent

    [SerializeField, Header("プレイヤーのデータ")]
    private PlayerWindow playerWindow; // プレイヤーのウィンドウデータ

    [SerializeField, Header("入れ替えに必要なコンポーネント群")]
    private Camera mainCamera; // メインカメラ
    [SerializeField]
    private Camera effectCamera;
    [SerializeField]
    private GameObject effectLayerObject; // プレイヤーのレイヤーマスク

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
