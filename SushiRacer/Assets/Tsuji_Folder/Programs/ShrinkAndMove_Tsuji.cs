using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class ShrinkAndMove_Tsuji : MonoBehaviour
{
    [SerializeField]
    private RectTransform targetRect;

    [SerializeField]
    private Vector2 targetPosition = new Vector2(100, 100); // �ڕW�ʒu
    [SerializeField]
    private Vector2 targetSize = new Vector2(50, 50); // �ڕW�T�C�Y
    [SerializeField]
    private float speed = 1f; // �ړ����x

    [SerializeField]
    private SequentialOperation_Tsuji soParent; // SequentialOperation_Tsuji�̎Q��

    [SerializeField]
    private SealPercent mySeal = SealPercent.None;

    private bool isMoving = false; // �ړ������ǂ����̃t���O
    private bool beforeMoving = false; // �ړ��O�̏�Ԃ�ۑ����邽�߂̃t���O

    private Vector2 firstPosition; // �����ʒu��ۑ����邽�߂̕ϐ�
    private Vector2 firstSize; // �����T�C�Y��ۑ����邽�߂̕ϐ�

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPosition = transform.position; // �����ʒu��ۑ�
        firstSize = transform.localScale;
    }

    void FixedUpdate()
    {
        if (isMoving == true)
        {
            Paste();
            soParent.SetNowPasteSeal(mySeal); // SequentialOperation_Tsuji�̃��\�b�h���Ăяo��
            soParent.SequentialOperationChildren(); // SequentialOperation_Tsuji�̃��\�b�h���Ăяo��
        }

        if (isMoving == false && beforeMoving == true)
        {
            if (soParent != null)
            {

            }
        }

        beforeMoving = isMoving; // �ړ��O�̏�Ԃ�ۑ�
    }

    public void Paste()
    {
        if (isMoving == true && beforeMoving == false)
        {
            TransformReset(); // �ړ��J�n����Transform�����Z�b�g
        }

        // �ʒu����
        targetRect.anchoredPosition = Vector2.Lerp(
        targetRect.anchoredPosition,
        targetPosition,
        Time.deltaTime * speed
    );

        // �X�P�[������
        targetRect.localScale = Vector2.Lerp(
            targetRect.localScale,
            targetSize,
            Time.deltaTime * speed
        );

        // ���B����i�덷�����e�j
        float posDiff = Vector2.Distance(targetRect.anchoredPosition, targetPosition);
        float scaleDiff = Vector2.Distance(targetRect.localScale, targetSize);

        if (posDiff < 0.1f && scaleDiff < 0.01f)
        {
            isMoving = false;

            // �ŏI�l�𖾎��I�ɃZ�b�g�i�덷�h�~�j
            targetRect.anchoredPosition = targetPosition;
            targetRect.localScale = targetSize;

        }
    }

    public void SetMovingFlag(bool flag)
    {
        isMoving = flag; // �ړ����J�n
    }

    public void TransformReset()
    {
        transform.position = firstPosition; // �����ʒu�ɖ߂�
        transform.localScale = firstSize; // �����T�C�Y�ɖ߂�
    }

    public SealPercent GetMySeal()
    {
        return mySeal; // ���݂�SealPercent��Ԃ�
    }
}
