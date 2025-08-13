using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackPoint
{
    public Vector3 worldPos; // 実際のコース上の位置
    public Vector2 uiPos;    // ミニマップUI上の位置（anchoredPosition）
}


public class MapMove_Tsuji : MonoBehaviour
{
    [SerializeField]
    private List<TrackPoint> trackPoints; // コース上のポイントリスト
    [SerializeField]
    private RectTransform playerIcon; // ミニマップ上のプレイヤーアイコン
    [SerializeField]
    private GameObject player; // プレイヤーオブジェクト（位置を取得するため）

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        float progress = GetLapProgress(player.transform.position, trackPoints);
        UpdateMiniMapIcon(playerIcon, trackPoints, progress);
    }


    float GetLapProgress(Vector3 playerPos, List<TrackPoint> track)
    {
        float totalLength = 0f;
        for (int i = 0; i < track.Count - 1; i++)
            totalLength += Vector3.Distance(track[i].worldPos, track[i + 1].worldPos);

        // 最も近いポイントを探す
        int nearest = 0;
        float nearestDist = float.MaxValue;
        for (int i = 0; i < track.Count; i++)
        {
            float dist = Vector3.Distance(playerPos, track[i].worldPos);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = i;
            }
        }

        // 次のポイント（ループする場合あり）
        int next = (nearest + 1) % track.Count;
        float segLength = Vector3.Distance(track[nearest].worldPos, track[next].worldPos);
        float segProgress = Vector3.Distance(track[nearest].worldPos, playerPos) / segLength;

        // 累積距離
        float traveled = 0f;
        for (int i = 0; i < nearest; i++)
            traveled += Vector3.Distance(track[i].worldPos, track[i + 1].worldPos);
        traveled += segLength * Mathf.Clamp01(segProgress);

        return traveled / totalLength; // 0～1
    }

    void UpdateMiniMapIcon(RectTransform icon, List<TrackPoint> track, float progress)
    {
        int segCount = track.Count - 1;
        float segmentT = progress * segCount;

        int segIndex = Mathf.FloorToInt(segmentT);
        float localT = segmentT - segIndex;

        Vector2 pos = Vector2.Lerp(track[segIndex].uiPos, track[(segIndex + 1) % track.Count].uiPos, localT);
        icon.anchoredPosition = pos;
    }

}
