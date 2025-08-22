
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent( typeof( Rigidbody ) )]
public class SplineAnimateRigidbody : MonoBehaviour
{
    [SerializeField]
    private SushiComponent sushiComponent = null; // 対象のSushiComponent
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

    [SerializeField, Header( "スパークエフェクト" )]
    private GameObject sparkEffect;

    [SerializeField, Header( "電撃の場所" )]
    private float sparkPositionX;


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

    private float offsetPsitionY = 0f; // Y軸の補正角度
    public float OffsetPsitionY
    {
        get => offsetPsitionY;
        set { offsetPsitionY = value; }
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

    private int derayFreamCount = 0; // フレームカウント（デバッグ用）

    // 追加: PlayFromClosestPoint開始時のパス上の回転と接続対象の回転
    private Quaternion initialPathRotation = Quaternion.identity;
    private Quaternion initialConnectedRotation = Quaternion.identity;

    // 追加: 回転補間用の係数
    [SerializeField, Header( "回転補間係数" )]
    private float rotationSmoothing = 10f;

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
        if (derayFreamCount < 600)
        {
            // 初期化後のフレームは処理をスキップ
            derayFreamCount++;
        }

        if (!isMoving || splineContainer == null || splineContainer.Spline == null)
            return;

        float splineLength = splineContainer.Spline.GetLength();
        if (splineLength == 0f)
        {
            Debug.LogWarning( "Spline length is zero, cannot update position." );
            return;
        }

        distance += Mathf.Abs( speedFactor * Time.fixedDeltaTime ) / splineLength;

        //進行方向が逆の場合は距離をマイナスにする
        if (isReversing)
        {
            //distance += ( speedFactor * Time.fixedDeltaTime ) / splineLength;
        }
        else
        {
            //distance -= ( speedFactor * Time.fixedDeltaTime ) / splineLength;
        }
        if (isLooping)
        {
            if (distance >= 1f)
            {
                distance -= 1f;
            }
            else if (distance < 0f)
            {
                distance += 1f;
            }
        }
        else
        {
            if (distance < 0f || distance > 1f)
            {
                StopMovement();
                sushiComponent.SetSushiMode( SushiMode.Normal );
                return;
            }
        }

        UpdateAtCurrentDistance();

        if (joint.connectedBody != null)
        {
            Debug.DrawLine( rb.position, joint.connectedBody.position, Color.red );
        }
    }

    /// <summary>
    /// 指定されたSplineContainerに基づき、オブジェクトの現在位置から最も近い曲線位置で移動を開始します。
    /// </summary>
    /// <param name="newSplineContainer">使用するSplineContainer</param>
    public bool PlayFromClosestPoint( SplineContainer newSplineContainer, Rigidbody rigidbody = null, float connecteDistance = 0.0f )
    {
        if (isMoving || splineContainer == newSplineContainer && derayFreamCount < 30)
        {
            return false;
        }

        if (newSplineContainer == null || newSplineContainer.Spline == null)
        {
            Debug.LogError( "Provided SplineContainer or its Spline is not valid." );
            return false;
        }

        splineContainer = newSplineContainer;
        offsetPsitionY = 0f;

        if (rigidbody != null)
        {
            SetJoint( rigidbody, connecteDistance );
        }

        distance = FindClosestParameter( rigidbody != null ? rigidbody.position : transform.position );
        UpdateAtCurrentDistance();

        // 追加: 初期状態の保存
        if (rigidbody != null)
        {
            initialConnectedRotation = rigidbody.rotation;
        }
        initialPathRotation = Quaternion.LookRotation( splineContainer.Spline.EvaluateTangent( distance ), Vector3.up );

        sparkEffect.SetActive( true );

        if ( isReversing )
        {
            sparkEffect.transform.localPosition = Vector3.right * sparkPositionX + Vector3.up * 0.24f;
        }
        else
        {
            sparkEffect.transform.localPosition = -Vector3.right * sparkPositionX + Vector3.up * 0.24f;
        }

        isMoving = true;
        return true;
    }

    /// <summary>
    /// スプライン上の移動を停止します。
    /// 現在の位置にオブジェクトを合わせ、更新を停止します。
    /// </summary>
    public void StopMovement()
    {
        UpdateAtCurrentDistance();
        derayFreamCount = 0;
        joint.connectedBody = null;
        sparkEffect.SetActive( false );
        isMoving = false;
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

        rb.MovePosition( targetPosition );

        Vector3 forward = splineContainer.Spline.EvaluateTangent( distance );
        if (forward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation( forward, Vector3.up );
            rb.MoveRotation( targetRotation );

            if (joint.connectedBody != null)
            {
                if (orientToPath)
                {
                    joint.connectedBody.rotation = Quaternion.Euler( 0f, offsetPsitionY, 0f ) * targetRotation;
                }
                else
                {
                    // 初期のパス回転との差分(delta)を計算
                    Quaternion delta = Quaternion.Inverse( initialPathRotation ) * targetRotation;
                    // 目標の回転を計算
                    Quaternion desiredRotation = Quaternion.Euler( 0f, offsetPsitionY, 0f ) * delta * initialConnectedRotation;
                    // Slerpでスムーズに補間
                    joint.connectedBody.rotation = Quaternion.Slerp( joint.connectedBody.rotation, desiredRotation, Time.fixedDeltaTime * rotationSmoothing );
                }
            }
        }
    }

    public void SetJoint( Rigidbody targetRigidbody, float connecteDistance = default )
    {
        if (targetRigidbody != null && splineContainer != null && splineContainer.Spline != null)
        {
            rb.position = Vector3.right * connecteDistance;
            joint.anchor = Vector3.right * connecteDistance;
            isReversing = connecteDistance < 0f;
        }
        joint.connectedBody = targetRigidbody;
    }
}