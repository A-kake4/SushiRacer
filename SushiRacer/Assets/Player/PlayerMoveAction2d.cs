using UnityEngine;
using Action2d;
using System.Collections.Generic;

[RequireComponent( typeof( Rigidbody2D ) )]
[RequireComponent( typeof( BoxCollider2D ) )]

public partial class PlayerMoveAction2d : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private Rigidbody2D m_rb;

    [SerializeField, Header("各種プレイヤー能力")]
    private PlayerMoveData m_playerData = new();

    [SerializeField, Header("各種プレイヤー音")]
    private PlayerAudioData m_playerAudioData = new();
    
    [SerializeField, Header("アニメーション")]
    private Animator m_animator;

    private float m_jumpDelayTime = 0.0f;
    private bool m_jumpInputOld = false;
    private bool m_jumpInput = false;
    private bool m_isJumping = false;
    public bool IsJumping => m_isJumping;

    private bool m_falling = false;
    public bool IsFalling => m_falling;

    private bool m_isMove = false;
    public bool IsMove => m_isMove;

    private bool m_isRight = false;
    public bool IsRight => m_isRight;

    private Vector2 m_totalLinearVelocity = Vector2.zero;
    private float m_moveInputX;

    private void Reset()
    {
        // Rigidbody2Dの取得
        m_rb = GetComponent<Rigidbody2D>();
        if ( m_rb == null )
        {
            m_rb = gameObject.AddComponent<Rigidbody2D>();
        }
        // Rigidbody2Dの設定
        m_rb.gravityScale = 1.0f;
        m_rb.freezeRotation = true;
        // Animatorの取得
        m_animator = GetComponent<Animator>();
    }

    public void AddVelocity( Vector2 _velocity )
    {
        // 速度を加算する
        m_totalLinearVelocity += _velocity;
    }

    private void Update()
    {
        HandleInput(); // 入力処理
    }

    private void FixedUpdate()
    {
        HandleMovement();       // 物理演算の更新
        HandleJump();           // ジャンプの処理
        CheckGround();          // 接触判定の処理
        UpdateAnimator();       // アニメーションの更新
        ApplyVelocity();        // 速度の適用
    }

    public void PlayerDeath()
    {
        // プレイヤーが死亡した場合の処理
        InputManager.Instance.CurrentGameMode = "Event";

        m_animator.SetTrigger( "isDeath" );
    }

    private void HandleInput()
    {
        if(InputManager.Instance.CurrentGameMode != "MainGame")
        {
            return;
        }

        // -----------------------------------------
        // 移動入力の受付
        m_moveInputX = InputManager.Instance.GetActionValue<Vector2>( "MainGame", "Move" ).x;

        //-----------------------------------------
        // ジャンプ入力の受付
        m_jumpInputOld = m_jumpInput;
        m_jumpInput = InputManager.Instance.GetActionValue<bool>( "MainGame", "Jump" );

        if ( m_jumpInput && !m_falling && !m_jumpInputOld )
        {
            AddVelocity( m_playerData.m_initialJumpForce * Vector2.up );

            m_isJumping = true;
            m_jumpDelayTime = 0.0f;

            //m_playerAudioData.m_jumpSe.Play();
        }
        //-----------------------------------------
    }

    private void HandleMovement()
    {
        // 移動量を1 or -1にする
        if ( m_moveInputX > 0.0f )
        {
            m_moveInputX = 1.0f;
        }
        else if ( m_moveInputX < 0.0f )
        {
            m_moveInputX = -1.0f;
        }

        // 移動中かどうかをアニメーションに反映
        m_isMove = m_moveInputX != 0.0f;

        if ( m_isMove )
        {
            m_isRight = m_moveInputX > 0.0f;
        }

        AddVelocity( m_playerData.m_speed * m_moveInputX * Vector2.right );
    }

    private void HandleJump()
    {
        if ( m_isJumping && m_jumpInput )
        {
            m_jumpDelayTime += Time.fixedDeltaTime;

            if ( m_jumpDelayTime <= m_playerData.m_jumpDelayTimeMax )
            {
                // ジャンプの持続力を計算
                float sustainedForce = m_playerData.m_sustainedJumpForce
                                        * Mathf.Sin(Mathf.PI / 2 * (1 - m_jumpDelayTime / m_playerData.m_jumpDelayTimeMax));
                m_rb.AddForce( sustainedForce * Vector2.up );
            }
            else
            {
                m_isJumping = false;
            }
        }
        else
        {
            m_isJumping = false;
            m_jumpDelayTime = 0.0f;
        }
    }

    private void CheckGround()
    {
        // 地面判定を行う
        Collider2D hit = Physics2D.OverlapBox(
            m_playerData.m_groundCheckPosition + (Vector2)transform.position,
            new Vector2(m_playerData.m_groundCheckWidth, m_playerData.m_groundCheckDistance),
            0.0f,
            m_playerData.m_groundLayerMask
        );

        if ( hit != null && m_falling )
        {
            m_isJumping = false;
        }
        m_falling = hit == null;
    }

    private void UpdateAnimator()
    {
        if ( m_animator == null )
        {
            Debug.LogError( "Animatorが設定されていません" );
            return;
        }

        // 地面にいるかどうかをアニメーションに反映
        m_animator.SetBool( "IsGround", !m_falling );

        // ジャンプ中かどうかをアニメーションに反映
        m_animator.SetBool( "IsJump", m_isJumping );

        // 移動中かどうかをアニメーションに反映
        m_animator.SetBool( "IsMove", m_isMove );

        // 移動方向をアニメーションに反映
        if ( m_isMove )
        {
            // アニメーションの向きを変更
            m_animator.transform.localRotation = Quaternion.Euler( 0.0f, m_isRight ? 180.0f : 0.0f, 0.0f );
        }
    }

    private void ApplyVelocity()
    {
        var velocity = m_rb.linearVelocity;
        velocity.x = m_totalLinearVelocity.x;
        velocity.y = Mathf.Max( velocity.y + m_totalLinearVelocity.y, m_playerData.m_fallSpeedMin );
        m_rb.linearVelocity = velocity;

        if ( velocity.y < 0 )
        {
            m_isJumping = false;
        }

        m_totalLinearVelocity = Vector2.zero;
    }

#if UNITY_EDITOR

    [SerializeField, Header("予測線の設定")]
    private bool drawPrediction = false; // 予測線を描画するかどうか
    [SerializeField]
    private int predictionSteps = 500; // 予測線の分割数
    [SerializeField]
    private bool predictionIsRight = false; // 予測線の方向（右向きかどうか）
    [SerializeField]
    private float predictionTime = 3.0f; // 予測線の合計時間
    [SerializeField]
    private Color maxJumpColor = Color.green; // 最大ジャンプ用の色
    [SerializeField]
    private Color minJumpColor = Color.blue; // 最小ジャンプ用の色

    private void OnDrawGizmosSelected()
    {
        // 既存の地面判定範囲描画
        if ( m_playerData.m_groundCheckPosition != Vector2.zero )
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube( 
                (Vector3)m_playerData.m_groundCheckPosition + transform.position, 
                new Vector3(
                            m_playerData.m_groundCheckWidth,
                            m_playerData.m_groundCheckDistance
                            )
                );
        }

        // 予測線の描画
        if ( drawPrediction )
        {
            DrawJumpPrediction();
        }
    }

    private void DrawJumpPrediction()
    {
        if ( m_rb == null || m_playerData == null )
            return;

        // Rigidbody2D のパラメータ取得
        float gravity = Physics2D.gravity.y * m_rb.gravityScale;
        float mass = m_rb.mass;
        float speed = m_playerData.m_speed;
        float initialJumpForce = m_playerData.m_initialJumpForce;
        float jumpDelayMax = m_playerData.m_jumpDelayTimeMax;
        float sustainedJumpForce = m_playerData.m_sustainedJumpForce;

        // 初期速度の計算
        float initialJumpVelocity = initialJumpForce / mass;

        float directionMultiplier = predictionIsRight ? 1.0f : -1.0f;

        // 最大ジャンプの予測
        Vector2 maxInitialVelocity = new Vector2(speed * directionMultiplier, initialJumpVelocity);
        List<Vector2> maxJumpPositions = PredictJump(maxInitialVelocity, gravity, jumpDelayMax, sustainedJumpForce);

        // 最小ジャンプの予測
        Vector2 minInitialVelocity = new Vector2(speed * directionMultiplier, initialJumpVelocity);
        List<Vector2> minJumpPositions = PredictJump(minInitialVelocity, gravity, 0.0f, 0.0f);

        // 最大ジャンプの描画
        if ( maxJumpPositions.Count >= 2 )
        {
            Gizmos.color = maxJumpColor;
            for ( int i = 0; i < maxJumpPositions.Count - 1; i++ )
            {
                Gizmos.DrawLine( maxJumpPositions[i], maxJumpPositions[i + 1] );
            }
        }

        // 最小ジャンプの描画
        if ( minJumpPositions.Count >= 2 )
        {
            Gizmos.color = minJumpColor;
            for ( int i = 0; i < minJumpPositions.Count - 1; i++ )
            {
                Gizmos.DrawLine( minJumpPositions[i], minJumpPositions[i + 1] );
            }
        }
    }

    /// <summary>
    /// ジャンプの予測位置を計算する
    /// </summary>
    /// <param name="initialVelocity">初期速度</param>
    /// <param name="gravity">重力加速度</param>
    /// <param name="sustainTime">持続ジャンプ時間</param>
    /// <param name="sustainedJumpForce">持続ジャンプ力</param>
    /// <returns>予測位置リスト</returns>
    private List<Vector2> PredictJump( Vector2 initialVelocity, float gravity, float sustainTime, float sustainedJumpForce )
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 position = m_playerData.m_groundCheckPosition + (Vector2)transform.position;
        Vector2 velocity = initialVelocity;
        float timeStep = predictionTime / predictionSteps;
        float elapsedTime = 0.0f;

        float directionMultiplier = predictionIsRight ? 1.0f : -1.0f;
        position += directionMultiplier * m_playerData.m_groundCheckWidth / 2 * Vector2.right;


        // 予測線の初期位置を追加
        positions.Add( position );

        for ( int i = 0; i < predictionSteps; i++ )
        {
            if ( elapsedTime <= sustainTime && sustainedJumpForce > 0 )
            {
                // 持続ジャンプ力の適用
                float appliedForce = sustainedJumpForce * Mathf.Sin(Mathf.PI / 2 * (1 - elapsedTime / sustainTime));
                velocity += new Vector2( 0, appliedForce / m_rb.mass ) * timeStep;
            }

            // 物理計算
            velocity += new Vector2( 0, gravity ) * timeStep;
            position += velocity * timeStep;
            elapsedTime += timeStep;

            // 地面判定
            Collider2D hit = Physics2D.OverlapCircle(position, m_playerData.m_groundCheckDistance, m_playerData.m_groundLayerMask);
            if ( hit != null )
            {
                positions.Add( position );
                break;
            }

            positions.Add( position );
        }

        return positions;
    }

#endif
}
