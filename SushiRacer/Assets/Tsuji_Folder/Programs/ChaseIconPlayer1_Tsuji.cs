using UnityEngine;
using UnityEngine.Rendering;

public class ChaseIconPlayer1_Tsuji : MonoBehaviour
{
    //[SerializeField]
    //private PlayerKeeper_Tsuji playerKeeper; // PlayerKeeperの参照

    [SerializeField]
    private float offsetHeight = 0.0f;
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private SushiDataScriptableObject sushi;

    private bool setUp = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerKeeper_Tsuji playerKeeper = PlayerKeeper_Tsuji.instance; // PlayerKeeperのインスタンスを取得
        if (playerKeeper != null)
        {
            GameObject player1 = playerKeeper.GetPlayer1(); // PlayerKeeperからプレイヤー1のGameObjectを取得
            if (player1 != null)
            {
                // プレイヤー1の位置にアイコンを追従させる
                transform.position = player1.transform.position + Vector3.up * offsetHeight;


                if ( !setUp )
                {
                    int charIndex = PlayerSelectManager.Instance.GetSelectedCharacterIndex(0);
                    meshRenderer.material = sushi.items[charIndex].minimapMaterial;
                    setUp = true;
                }
            }
        }

    }
}
