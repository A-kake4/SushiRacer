using UnityEngine;

[System.Serializable]
public class RacerProgress_Tsuji : MonoBehaviour
{
    private int currentWaypointIndex;
    private float totalDistance;
    private int rank;
    private Transform targetObject; // 参照元のオブジェクト
    [SerializeField]
    private float heightOffset = 5f; // 上方向へのオフセット
    [SerializeField]
    private Vector3 startPoint = Vector3.zero;  // スタート地点

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

    public void Teleport()
    {
        if (targetObject != null)
        {
            Vector3 newPosition = targetObject.position + Vector3.up * heightOffset;
            transform.position = newPosition;
        }
        else
        {
            Vector3 newPosition = startPoint + Vector3.up * heightOffset;
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "WayPoint")
        {
            targetObject = collider.transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "WayPoint")
        {
            targetObject = collision.transform;
        }
    }
}
