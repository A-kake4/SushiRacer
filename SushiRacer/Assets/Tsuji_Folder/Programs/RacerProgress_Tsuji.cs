using UnityEngine;

[System.Serializable]
public class RacerProgress_Tsuji : MonoBehaviour
{
    private int currentWaypointIndex;
    private float totalDistance;
    private int rank;
    private Transform targetObject; // �Q�ƌ��̃I�u�W�F�N�g
    [SerializeField]
    private float heightOffset = 5f; // ������ւ̃I�t�Z�b�g

    private void Update()
    {
        Debug.Log(gameObject.name + " �͌��݂̏��ʂ́F" + rank + " ��");
    }

    public void UpdateProgress(Transform[] waypoints)
    {
        float distance = 0f;
        for (int i = 0; i < currentWaypointIndex; i++)
        {
            distance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }
        distance += Vector3.Distance(waypoints[currentWaypointIndex].position, transform.position);
        totalDistance = distance;
    }

    public void SetRank(int _rank)
    {
        rank = _rank;
    }

    public float GetTotalDistance()
    {
        return totalDistance;
    }

    void Teleport()
    {
        if (targetObject != null)
        {
            Vector3 newPosition = targetObject.position + Vector3.up * heightOffset;
            transform.position = newPosition;
        }
    }

}
