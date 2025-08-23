using UnityEngine;

public class SushiButton : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0; // プレイヤーのインデックス
    public int PlayerIndex
    {
        get => playerIndex;
        set => playerIndex = value;
    }

    [SerializeField] private int sushiIndex = 0; // 寿司のインデックス
    public int SushiIndex
    {
        get => sushiIndex;
        set => sushiIndex = value;
    }

    // ステージセレクトマネージャーの参照
    private void OnMouseDown()
    {
        SetPlayerSelect(  );
    }

    public void SetPlayerSelect(  )
    {
        PlayerSelectManager.Instance.SetSelectedCharacterIndex( playerIndex, sushiIndex );
    }
}
