using Unity.VisualScripting;
using UnityEngine;

public class SinMovePosition : MonoBehaviour
{
    private Transform myTransform;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 5f;

    private float time;

    private bool isMoving = false;

    private bool delayDestroy = false;
    public bool DelayDestroy
    {
        get { return delayDestroy; }
        set { delayDestroy = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTransform = this.transform;
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
        SetMove( targetPos, myTransform.position );
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        time += Time.fixedDeltaTime * moveSpeed;
        float t = Mathf.Sin(time * Mathf.PI * 0.5f); // 0����1�܂ł�sin�g�ŕ��
        myTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
        if (t >= 1f)
        {
            myTransform.position = targetPosition; // �ŏI�ʒu���m���ɐݒ�
            isMoving = false; // �ړ�����
            if (delayDestroy)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
