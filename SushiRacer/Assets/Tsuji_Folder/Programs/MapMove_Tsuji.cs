using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackPoint
{
    public Vector3 worldPos; // ���ۂ̃R�[�X��̈ʒu
    public Vector2 uiPos;    // �~�j�}�b�vUI��̈ʒu�ianchoredPosition�j
}


public class MapMove_Tsuji : MonoBehaviour
{
    [SerializeField]
    private List<TrackPoint> trackPoints; // �R�[�X��̃|�C���g���X�g
    [SerializeField]
    private RectTransform playerIcon; // �~�j�}�b�v��̃v���C���[�A�C�R��
    [SerializeField]
    private GameObject player; // �v���C���[�I�u�W�F�N�g�i�ʒu���擾���邽�߁j

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

        // �ł��߂��|�C���g��T��
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

        // ���̃|�C���g�i���[�v����ꍇ����j
        int next = (nearest + 1) % track.Count;
        float segLength = Vector3.Distance(track[nearest].worldPos, track[next].worldPos);
        float segProgress = Vector3.Distance(track[nearest].worldPos, playerPos) / segLength;

        // �ݐϋ���
        float traveled = 0f;
        for (int i = 0; i < nearest; i++)
            traveled += Vector3.Distance(track[i].worldPos, track[i + 1].worldPos);
        traveled += segLength * Mathf.Clamp01(segProgress);

        return traveled / totalLength; // 0�`1
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
