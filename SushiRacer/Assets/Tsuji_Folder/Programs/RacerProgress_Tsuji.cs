using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RacerProgress_Tsuji : MonoBehaviour
{
    private int currentWaypointIndex;
    private float totalDistance;
    private int rank;
    private int beforeRank;

    private Transform targetObject; // �Q�ƌ��̃I�u�W�F�N�g
    [SerializeField]
    private float heightOffset = 5f; // ������ւ̃I�t�Z�b�g
    [SerializeField]
    private Vector3 startPoint = Vector3.zero;  // �X�^�[�g�n�_

    [SerializeField]
    private Image playerRankImage; // �v���C���[���ʗp��UI�C���[�W

    [SerializeField]
    private Sprite rank1Sprite; // �P�ʂ̃X�v���C�g

    [SerializeField]
    private Sprite rank2Sprite; // �Q�ʂ̃X�v���C�g

    private void FixedUpdate()
    {
        if (beforeRank != rank && playerRankImage != null && rank1Sprite != null && rank2Sprite != null)
        {
            if (rank == 1)
            {
                playerRankImage.sprite = rank1Sprite; // �P�ʂ̃X�v���C�g��ݒ�
            }
            else if (rank == 2)
            {
                playerRankImage.sprite = rank2Sprite; // �Q�ʂ̃X�v���C�g��ݒ�
            }
        }
        beforeRank = rank;
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

    public int GetRank()
    {
        return rank;
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
