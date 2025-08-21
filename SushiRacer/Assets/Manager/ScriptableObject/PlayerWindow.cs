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
    [Header("�����̈�")]
    public Rect viewportRect;

    [Header("�G�t�F�N�g���C���[")]
    public LayerMask effectLayer;
}