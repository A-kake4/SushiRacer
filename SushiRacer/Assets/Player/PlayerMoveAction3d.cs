using UnityEngine;
using Action3d;
using System.Collections.Generic;

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( CapsuleCollider ) )]

public partial class PlayerMoveAction3d : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private Rigidbody m_rb;

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

    private Vector3 m_totalLinearVelocity = Vector3.zero;
    private Vector3 m_moveInput;

    private void Reset()
    {
        // Rigidbodyの取得
        m_rb = GetComponent<Rigidbody>();
        if ( m_rb == null )
        {
            m_rb = gameObject.AddComponent<Rigidbody>();
        }
        // Rigidbodyの設定
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Animatorの取得
        m_animator = GetComponent<Animator>();
    }

    public void AddVelocity( Vector3 _velocity )
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
        if ( InputManager.Instance.CurrentGameMode != "MainGame" )
        {
            return;
        }

        // -----------------------------------------
        // 移動入力の受付
        Vector2 moveInput2D = InputManager.Instance.GetActionValue<Vector2>(1, "MainGame", "Move");
        m_moveInput.x = moveInput2D.x;
        m_moveInput.z = moveInput2D.y;

        //-----------------------------------------
        // ジャンプ入力の受付
        m_jumpInputOld = m_jumpInput;
        m_jumpInput = InputManager.Instance.GetActionValue<bool>(1, "MainGame", "Jump" );

        if ( m_jumpInput && !m_falling && !m_jumpInputOld )
        {
            AddVelocity( m_playerData.m_initialJumpForce * Vector3.up );

            m_isJumping = true;
            m_jumpDelayTime = 0.0f;

            //m_playerAudioData.m_jumpSe.Play();
        }
        //-----------------------------------------
    }

    private void HandleMovement()
    {
        // 移動中かどうかをアニメーションに反映
        m_isMove = m_moveInput != Vector3.zero;

        AddVelocity( m_playerData.m_speed * m_moveInput );
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
                m_rb.AddForce( sustainedForce * Vector3.up );
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
        Vector3 groundCheckPosition = m_playerData.m_groundCheckPosition + transform.position;
        Vector3 groundCheckSize = new(m_playerData.m_groundCheckWidth, m_playerData.m_groundCheckDistance, m_playerData.m_groundCheckWidth);
        Collider[] hitColliders = Physics.OverlapBox(
            groundCheckPosition,
            groundCheckSize / 2,
            Quaternion.identity,
            m_playerData.m_groundLayerMask
        );

        if ( hitColliders.Length > 0 && m_falling )
        {
            m_isJumping = false;
        }

        m_falling = hitColliders.Length == 0;
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
        }
    }

    private void ApplyVelocity()
    {
        Vector3 velocity = m_rb.linearVelocity;
        velocity.x = m_totalLinearVelocity.x;
        velocity.y = Mathf.Max( velocity.y + m_totalLinearVelocity.y, m_playerData.m_fallSpeedMin );
        velocity.z = m_totalLinearVelocity.z;
        m_rb.linearVelocity = velocity;

        if ( velocity.y < 0 )
        {
            m_isJumping = false;
        }

        m_totalLinearVelocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // 地面判定の範囲を可視化
        Vector3 groundCheckPosition = m_playerData.m_groundCheckPosition + transform.position;
        Vector3 groundCheckSize = new(m_playerData.m_groundCheckWidth, m_playerData.m_groundCheckDistance, m_playerData.m_groundCheckWidth);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube( groundCheckPosition, groundCheckSize );
    }
}
