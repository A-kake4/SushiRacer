using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class ShrinkAndMove_Tsuji : MonoBehaviour
{
    [SerializeField]
    private RectTransform targetRect;

    [SerializeField]
    private Vector2 targetPosition = new Vector2(100, 100); // 目標位置
    [SerializeField]
    private Vector2 targetSize = new Vector2(50, 50); // 目標サイズ
    [SerializeField]
    private float speed = 1f; // 移動速度

    [SerializeField]
    private SequentialOperation_Tsuji soParent; // SequentialOperation_Tsujiの参照

    [SerializeField]
    private SealPercent mySeal = SealPercent.None;

    private bool isMoving = false; // 移動中かどうかのフラグ
    private bool beforeMoving = false; // 移動前の状態を保存するためのフラグ

    private Vector2 firstPosition; // 初期位置を保存するための変数
    private Vector2 firstSize; // 初期サイズを保存するための変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPosition = transform.position; // 初期位置を保存
        firstSize = transform.localScale;
    }

    void FixedUpdate()
    {
        if (isMoving == true)
        {
            Paste();
            soParent.SetNowPasteSeal(mySeal); // SequentialOperation_Tsujiのメソッドを呼び出す
            soParent.SequentialOperationChildren(); // SequentialOperation_Tsujiのメソッドを呼び出す
        }

        if (isMoving == false && beforeMoving == true)
        {
            if (soParent != null)
            {

            }
        }

        beforeMoving = isMoving; // 移動前の状態を保存
    }

    public void Paste()
    {
        if (isMoving == true && beforeMoving == false)
        {
            TransformReset(); // 移動開始時にTransformをリセット
        }

        // 位置を補間
        targetRect.anchoredPosition = Vector2.Lerp(
        targetRect.anchoredPosition,
        targetPosition,
        Time.deltaTime * speed
    );

        // スケールを補間
        targetRect.localScale = Vector2.Lerp(
            targetRect.localScale,
            targetSize,
            Time.deltaTime * speed
        );

        // 到達判定（誤差を許容）
        float posDiff = Vector2.Distance(targetRect.anchoredPosition, targetPosition);
        float scaleDiff = Vector2.Distance(targetRect.localScale, targetSize);

        if (posDiff < 0.1f && scaleDiff < 0.01f)
        {
            isMoving = false;

            // 最終値を明示的にセット（誤差防止）
            targetRect.anchoredPosition = targetPosition;
            targetRect.localScale = targetSize;

        }
    }

    public void SetMovingFlag(bool flag)
    {
        isMoving = flag; // 移動を開始
    }

    public void TransformReset()
    {
        transform.position = firstPosition; // 初期位置に戻す
        transform.localScale = firstSize; // 初期サイズに戻す
    }

    public SealPercent GetMySeal()
    {
        return mySeal; // 現在のSealPercentを返す
    }
}
