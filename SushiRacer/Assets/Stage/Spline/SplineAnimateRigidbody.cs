using UnityEngine;
using UnityEngine.Splines;

[RequireComponent( typeof( Rigidbody ) )]
public class SplineAnimateRigidbody : MonoBehaviour
{
    [SerializeField, Header( "スプラインに沿って移動するか" )]
    private bool awakeMove = true;

    // スプラインコンテナへの参照（インスペクターで設定）
    [SerializeField, Header( "挙動を合わせるスプライン" )]
    private SplineContainer splineContainer;
    public SplineContainer SplineContainer
    {
        get { return splineContainer; }
        set { splineContainer = value; }
    }

    [SerializeField, Header( "接続対象" )]
    private HingeJoint joint; // Rigidbodyと接続するためのJoint
    public HingeJoint Joint
    {
        get { return joint; }
    }

    // 進行パラメータの変化を調整する係数（Inspector上では補正用）
    [SerializeField, Header( "移動量調整用係数" )]
    private float speedFactor = 1f;
    public float SpeedFactor
    {
        get { return speedFactor; }
        set { speedFactor = value; }
    }

    // パスに合わせて回転させるかどうか
    [SerializeField, Header( "パスに合わせて回転させるか" )]
    private bool orientToPath = true;
    // オブジェクトの進行方向から見たオフセット
    [SerializeField, Header( "オブジェクト位置のオフセット" )]
    private Vector3 offsetPosition;
    public Vector3 OffsetPosition
    {
        get { return offsetPosition; }
        set { offsetPosition = value; }
    }
    [SerializeField, Header( "オブジェクト位置のオフセット" )]
    private float offsetRotation = 5f;
    public float OffsetRotation
    {
        get { return offsetRotation; }
        set { offsetRotation = value; }
    }

    [SerializeField, Header( "スプラインをループするか" )]
    private bool isLooping = false; // スプラインをループするか

    [SerializeField, Header( "進行方向が逆だった場合の修正用" )]
    private bool isReversing = false; // スプラインの進行方向を反転するかどうか（接地方向を間違えたとき用）

    // スプライン上の進行パラメータ（0～1と想定）
    [SerializeField, Header( "スプライン上の進行パラメータ" ), ReadOnly]
    private bool isMoving = false; // スプライン上を移動中かどうか
    public bool IsMoving
    {
        get { return isMoving; }
    }
    private float distance = 0f; // スプライン上の進行パラメータ（0～1）
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {

        if (awakeMove)
        {
            PlayFromClosestPoint( splineContainer );
            UpdateAtCurrentDistance();
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving || splineContainer == null || splineContainer.Spline == null)
            return;

        // オブジェクトの実際の速度に基づいて進行パラメータを更新（スプラインの長さで正規化）
        float splineLength = splineContainer.Spline.GetLength();
        if (splineLength == 0f)
        {
            Debug.LogWarning( "Spline length is zero, cannot update position." );
            return;
        }

        // 進行方向が逆の場合は距離をマイナスにする
        if (isReversing)
        {
            distance -= ( speedFactor * Time.fixedDeltaTime ) / splineLength;
        }
        else
        {
            distance += ( speedFactor * Time.fixedDeltaTime ) / splineLength;
        }

        // ループする場合はパラメータを0～1の範囲に制限
        if (isLooping)
        {
            if (distance >= 1f)
            {
                distance -= 1f; // ループする場合はパラメータを0～1の範囲に戻す
            }
            else if (distance < 0f)
            {
                distance += 1f; // 負の値の場合も同様に戻す
            }
        }
        else
        {
            distance = Mathf.Clamp01( distance );
            if (distance >= 1f || distance <= 0f)
            {
                isMoving = false; // ループしない場合は移動を停止
            }
        }

        // オブジェクトの位置と回転を更新
        UpdateAtCurrentDistance();

        // 接続対象との直線距離を表示する
        if (joint.connectedBody != null)
        {
            Debug.DrawLine( rb.position, joint.connectedBody.position, Color.red );
        }
    }

    /// <summary>
    /// 指定されたSplineContainerに基づき、オブジェクトの現在位置から最も近い曲線位置で移動を開始します。
    /// </summary>
    /// <param name="newSplineContainer">使用するSplineContainer</param>
    public void PlayFromClosestPoint( SplineContainer newSplineContainer, Rigidbody rigidbody = null, float connecteDistance = 0.0f )
    {
        if (newSplineContainer == null || newSplineContainer.Spline == null)
        {
            Debug.LogError( "Provided SplineContainer or its Spline is not valid." );
            return;
        }

        if (splineContainer == newSplineContainer && isMoving)
        {
            Debug.LogWarning( "Already moving on the specified spline." );
            return;
        }

        if (rigidbody != null)
        {
            SetJoint( rigidbody, connecteDistance );
        }

        splineContainer = newSplineContainer;
        distance = FindClosestParameter( rb.position );
        UpdateAtCurrentDistance();
        isMoving = true;
    }

    /// <summary>
    /// オブジェクトの位置からスプライン上の最も近い割合をサンプリングで求めます。
    /// </summary>
    /// <param name="point">対象のワールド座標</param>
    /// <returns>0～1の割合</returns>
    private float FindClosestParameter( Vector3 point )
    {
        float closestParam = 0f;
        float minDist = float.MaxValue;
        const int samples = 100;
        for (int i = 0; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 samplePos = splineContainer.Spline.EvaluatePosition( t );
            float dist = Vector3.Distance( point, samplePos );
            if (dist < minDist)
            {
                minDist = dist;
                closestParam = t;
            }
        }
        return closestParam;
    }

    /// <summary>
    /// 現在のdistanceに基づいて、オブジェクトの位置と回転を更新します。
    /// </summary>
    private void UpdateAtCurrentDistance()
    {
        Vector3 splinePos = splineContainer.Spline.EvaluatePosition( distance );
        Quaternion rot = orientToPath
            ? Quaternion.LookRotation( splineContainer.Spline.EvaluateTangent( distance ), Vector3.up )
            : Quaternion.identity;
        Vector3 targetPosition = splinePos + rot * offsetPosition;

        // 補間処理を削除し、確実にターゲット位置へ移動させる
        rb.MovePosition( targetPosition );

        if (orientToPath)
        {
            Vector3 forward = splineContainer.Spline.EvaluateTangent( distance );
            if (forward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation( forward, Vector3.up );
                rb.MoveRotation( targetRotation );

                // プレイヤーが向く方向に補正をかける
                Vector3 angle = targetRotation * ( joint.anchor * offsetRotation);

                targetRotation = Quaternion.LookRotation( forward + angle, Vector3.up );

                joint.connectedBody.rotation = targetRotation;
            }
        }
    }

    public void SetJoint( Rigidbody targetRigidbody, float connecteDistance = default )
    {
        if (targetRigidbody != null && splineContainer != null && splineContainer.Spline != null)
        {
            joint.anchor = Vector3.right * connecteDistance;

            isReversing = connecteDistance < 0f; // 接続距離が負の場合は進行方向を反転
        }

        joint.connectedBody = targetRigidbody;
    }
}
