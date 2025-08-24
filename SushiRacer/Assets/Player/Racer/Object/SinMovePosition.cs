using Unity.VisualScripting;
using UnityEngine;

public class SinMovePosition : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;

    [SerializeField, Header("移動速度")]
    private float moveSpeed = 5f;

    private float time;

    private bool isMoving = false;

    private bool delayDestroy = false;
    public bool DelayDestroy
    {
        get { return delayDestroy; }
        set { delayDestroy = value; }
    }

    public void SetMove(Vector3 targetPos, Vector3 startPos )
    {
        startPosition = startPos;
        targetPosition = targetPos;
        time = 0f;
        isMoving = true;
    }

    public void SetMove(Vector3 targetPos)
    {
        SetMove( targetPos, transform.position );
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        time += Time.fixedDeltaTime * moveSpeed;
        float t = Mathf.Sin(time * Mathf.PI * 0.5f); // 0から1までをsin波で補間
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        if (t >= 1f)
        {
            transform.position = targetPosition; // 最終位置を確実に設定
            isMoving = false; // 移動完了
            if (delayDestroy)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
