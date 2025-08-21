using UnityEngine;

[CreateAssetMenu( fileName = "PlayerData", menuName = "Manager/PlayerDatas", order = 0 )]
public class PlayerWindow : ScriptableObject
{
    public CameraData[] cameraDatas;

    public CameraData GetCameraDatas(int index)
    {
        return cameraDatas[index];
    }
}

[System.Serializable]
public class CameraData
{
    [Header("分割領域")]
    public Rect viewportRect;

    [Header("エフェクトレイヤー")]
    public LayerMask effectLayer;
}