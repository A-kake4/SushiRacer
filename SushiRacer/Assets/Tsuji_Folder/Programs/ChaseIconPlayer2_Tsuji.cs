using UnityEngine;

public class ChaseIconPlayer2_Tsuji : MonoBehaviour
{
    [SerializeField]
    private PlayerKeeper_Tsuji playerKeeper; // PlayerKeeperの参照

    [SerializeField]
    private float offsetHeight = 0.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerKeeper != null)
        {
            GameObject player2 = playerKeeper.GetPlayer2(); // PlayerKeeperからプレイヤー2のGameObjectを取得
            if (player2 != null)
            {
                // プレイヤー2の位置にアイコンを追従させる
                transform.position = player2.transform.position + Vector3.up * offsetHeight;
            }
        }
    }
}
