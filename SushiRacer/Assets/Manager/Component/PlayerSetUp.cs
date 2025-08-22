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

        // ゲームパッドの入力設定
        var device = PlayerSelectManager.Instance.GetPlayerDevice( playerNumber );
        if ( device == null )
        {
            Debug.LogError( $"プレイヤー{playerNumber}に割り当てられたデバイスが null です。" );
        }
        else
        {
            Debug.Log( $"プレイヤー{playerNumber}に割り当てられたデバイス: {device}" );
        }

        // 入力マネージャにデバイスを登録
        InputManager.Instance.RegisterPlayerDevice( playerNumber, device );
    }
}
