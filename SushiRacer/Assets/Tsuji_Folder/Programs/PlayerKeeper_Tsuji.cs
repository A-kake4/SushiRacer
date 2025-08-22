using UnityEngine;
using UnityEngine.UI;

public class PlayerKeeper_Tsuji : MonoBehaviour
{
    private GameObject player1 = null; // プレイヤー1のGameObject
    private GameObject player2 = null; // プレイヤー2のGameObject

    [SerializeField]
    private Image player1RankImage; // プレイヤー順位用のUIイメージ

    [SerializeField]
    private Image player2RankImage; // プレイヤー順位用のUIイメージ

    [SerializeField]
    private Sprite rank1Sprite; // １位のスプライト

    [SerializeField]
    private Sprite rank2Sprite; // ２位のスプライト

    [SerializeField]
    private CircleGauge_Tsuji circle1;
    public CircleGauge_Tsuji Circle1 => circle1;

    [SerializeField]
    private CircleGauge_Tsuji circle2;
    public CircleGauge_Tsuji Circle2 => circle2;


    private Camera camera1 = null; // プレイヤー1のカメラ
    private Camera camera2 = null; // プレイヤー2のカメラ

    public static PlayerKeeper_Tsuji  instance = null; // シングルトンインスタンス

    private void Awake()
    {
        // シングルトンパターンの実装
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    //private void FixedUpdate()
    //{
    //    if(player1 == null || player2 == null)
    //    {
    //        player1 = GameObject.Find("Player1"); // タグでプレイヤー1を検索
    //        player2 = GameObject.Find("Player2"); // タグでプレイヤー2を検索

    //        player1.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player1RankImage, rank1Sprite, rank2Sprite);
    //        player2.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player2RankImage, rank1Sprite, rank2Sprite);

    //    }

    //    if (player1 == null)
    //    {
    //        Debug.Log("プレイヤーの1");
    //    }

    //    if(player2 == null)
    //    {
    //        Debug.Log("プレイヤーの2");
    //    }
    //}

    public void SetPlayer1(GameObject player)
    {
        player1 = player; // プレイヤー1のGameObjectを設定
        player1.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player1RankImage,rank1Sprite,rank2Sprite);
    }

    public void SetCamera1(Camera cameraObject)
    {
        camera1 = cameraObject; // プレイヤー1のカメラを設定
    }

    public Camera GetCamera1()
    {
        return camera1; // プレイヤー1のカメラを取得
    }

    public void SetPlayer2(GameObject player)
    {
        player2 = player; // プレイヤー2のGameObjectを設定
        player2.GetComponent<RacerProgress_Tsuji>().SetRankImageAndRankSprite(player2RankImage, rank1Sprite, rank2Sprite);
    }
    public void SetCamera2(Camera cameraObject)
    {
        camera2 = cameraObject; // プレイヤー2のカメラを設定
    }

    public Camera GetCamera2()
    {
        return camera2; // プレイヤー2のカメラを取得
    }

    public GameObject GetPlayer1()
    {
        return player1; // プレイヤー1のGameObjectを取得
    }

    public GameObject GetPlayer2()
    {
        return player2; // プレイヤー2のGameObjectを取得
    }
}
