using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RacerProgress_Tsuji : MonoBehaviour
{
    private int currentWaypointIndex;
    private float totalDistance;
    private int rank;
    private int beforeRank;
    [SerializeField]
    private float wasabiSealPercentage;

    private Transform targetObject; // �Q�ƌ��̃I�u�W�F�N�g
    [SerializeField]
    private float heightOffset = 5f; // ������ւ̃I�t�Z�b�g
    [SerializeField]
    private Vector3 startPoint = Vector3.zero;  // �X�^�[�g�n�_

    [SerializeField]
    private Image playerRankImage; // �v���C���[���ʗp��UI�C���[�W

    [SerializeField]
    private Image playerWasabiSeal; // �v���C���[�̂킳�уV�[���pUI�C���[�W

    [SerializeField]
    private Sprite rank1Sprite; // �P�ʂ̃X�v���C�g

    [SerializeField]
    private Sprite rank2Sprite; // �Q�ʂ̃X�v���C�g

    [SerializeField]
    private Sprite wasabiSealSprite_30Percent; // �킳�уV�[���̃X�v���C�g�A30%

    [SerializeField]
    private Sprite wasabiSealSprite_50Percent; // �킳�уV�[���̃X�v���C�g�A50%

    [SerializeField]
    private Sprite wasabiSealSprite_100Percent; // �킳�уV�[���̃X�v���C�g�A100%

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

        if (playerWasabiSeal != null)
        {
            // UI���\��
            playerWasabiSeal.color = new Color(playerWasabiSeal.color.r, playerWasabiSeal.color.g, playerWasabiSeal.color.b, 1f);
            if (wasabiSealPercentage >= 100f)
            {
                playerWasabiSeal.sprite = wasabiSealSprite_100Percent; // 100%�̃X�v���C�g��ݒ�
            }
            else if (wasabiSealPercentage >= 50f)
            {
                playerWasabiSeal.sprite = wasabiSealSprite_50Percent; // 50%�̃X�v���C�g��ݒ�
            }
            else if (wasabiSealPercentage >= 30f)
            {
                playerWasabiSeal.sprite = wasabiSealSprite_30Percent; // 30%�̃X�v���C�g��ݒ�
            }
            else
            {
                // UI���\��
                playerWasabiSeal.color = new Color(playerWasabiSeal.color.r, playerWasabiSeal.color.g, playerWasabiSeal.color.b, 0f);
            }
        }
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

    public void SetWasabiSealPercentage(float percentage)
    {
        wasabiSealPercentage = Mathf.Clamp(percentage, 0f, 100f);
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
