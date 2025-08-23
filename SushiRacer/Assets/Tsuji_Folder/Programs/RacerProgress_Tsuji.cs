using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum CheckPointDir
{
    None,
    ForwardDirection,
    ReverseDirection,
}

[System.Serializable]
public class RacerProgress_Tsuji : MonoBehaviour
{
    private int currentWaypointIndex = 0;
    private float totalDistance;
    private int rank;
    private int beforeRank;
    private int checkPointCount = 0;    // 通過したチェックポイントの数

    private Transform targetObject; // 参照元のオブジェクト
    [SerializeField]
    private float heightOffset = 5f; // 上方向へのオフセット
    [SerializeField]
    private Vector3 startPoint = Vector3.zero;  // スタート地点

    CheckPointDir insertPattern = CheckPointDir.None;
    CheckPointDir exitPattern = CheckPointDir.None;

    [SerializeField]
    private Image playerRankImage; // プレイヤー順位用のUIイメージ

    [SerializeField]
    private Sprite rank1Sprite; // １位のスプライト

    [SerializeField]
    private Sprite rank2Sprite; // ２位のスプライト

    [SerializeField]
    private float returnHeight = -20f; // 復帰したい高さ

    private void FixedUpdate()
    { 
        if(rank1Sprite == null || rank2Sprite == null || playerRankImage == null)
        {
            Debug.Log("順位用の Image や Sprite が未設定です");

            return;
        }

        if (transform.position.y < returnHeight)
        {
            Teleport();
        }

        // チェックポイントの通過判定
        if (insertPattern == CheckPointDir.ForwardDirection && exitPattern == CheckPointDir.ForwardDirection)
        {
            checkPointCount++;

            insertPattern = CheckPointDir.None;
            exitPattern = CheckPointDir.None;
        }

        // 逆走してチェックポイントを通過した場合
        else if (insertPattern == CheckPointDir.ReverseDirection && exitPattern == CheckPointDir.ReverseDirection)
        {
            if(checkPointCount > 0)
            {
                checkPointCount--;
            }
            insertPattern = CheckPointDir.None;
            exitPattern = CheckPointDir.None;
        }

        if (beforeRank != rank && playerRankImage != null && rank1Sprite != null && rank2Sprite != null)
        {
            if (rank == 1)
            {
                playerRankImage.sprite = rank1Sprite; // １位のスプライトを設定
            }
            else if (rank == 2)
            {
                playerRankImage.sprite = rank2Sprite; // ２位のスプライトを設定
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
    public int GetCheckPointCount()
    {
        return checkPointCount;
    }

    public void Teleport()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 vel = rb.linearVelocity;

            // 落下速度をリセット
            vel.y = 0;
            rb.linearVelocity = vel;

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
    }

    public void SetRankImageAndRankSprite(Image rankImage, Sprite rank1,Sprite rank2)
    {
        playerRankImage = rankImage;
        rank1Sprite = rank1;
        rank2Sprite = rank2;
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject obj = collider.gameObject;
        if (obj.tag == "WayPoint")
        {
            targetObject = collider.transform;


            Vector3 wallPos = collider.transform.position;
            wallPos.y = 0.0f;

            Vector3 racerPos = transform.position;
            racerPos.y = 0.0f;

            Vector3 wallfromPlayerDir = racerPos - wallPos;

            float dot = Vector3.Dot(obj.transform.forward.normalized, wallfromPlayerDir.normalized);
            
            if(dot < 0.0f)
            {
                insertPattern = CheckPointDir.ForwardDirection;
            }
            else if(dot > 0.0f)
            {
                insertPattern = CheckPointDir.ReverseDirection;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        GameObject obj = collider.gameObject;
        if (obj.tag == "WayPoint")
        {
            targetObject = collider.transform;

            Vector3 wallPos = collider.transform.position;
            wallPos.y = 0.0f;

            Vector3 racerPos = transform.position;
            racerPos.y = 0.0f;

            Vector3 wallfromPlayerDir = racerPos - wallPos;

            float dot = Vector3.Dot(obj.transform.forward.normalized, wallfromPlayerDir.normalized);

            if (dot > 0.0f)
            {
                exitPattern = CheckPointDir.ForwardDirection;
            }
            else if (dot < 0.0f)
            {
                exitPattern = CheckPointDir.ReverseDirection;
            }
        }
    }
}
