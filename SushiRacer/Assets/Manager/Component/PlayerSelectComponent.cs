using UnityEngine;

public class PlayerSelectComponent : MonoBehaviour
{
    [SerializeField,Header("プレイヤー番号")]
    private int playerNumber = 0; // プレイヤー番号

    [SerializeField,Header("使用するキャラクターデータ")]
    private SushiDataScriptableObject playerData;

    [SerializeField, Header("出現位置のオフセット")]
    private Vector3 positionOffset;

    [SerializeField, ReadOnly]
    private int selectedCharacterIndex = 0; // 選択されたキャラクターのインデックス


    private void Start()
    {
        // 選んだキャラクターのインデックスを取得
        selectedCharacterIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex( playerNumber );

        var playerPrefab = playerData.GetItem(selectedCharacterIndex ).sushiPrefub;

        // この場所で生成
        var sushiObject = Instantiate( playerPrefab, transform.position + positionOffset, transform.rotation );

        var setUpData = sushiObject.GetComponent<PlayerSetUp>();
        setUpData.SetUpPlayer(playerNumber);
    }
}
