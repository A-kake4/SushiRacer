using UnityEngine;

namespace Action2d
{
    [System.Serializable]
    public class PlayerMoveData
    {
        [Header("移動速度")]
        public float m_speed = 5.0f;

        [Header("ジャンプ初速")]
        public float m_initialJumpForce = 5.0f;

        [Header("ジャンプ長押し時の継続的上昇速度")]
        public float m_sustainedJumpForce = 2.0f;

        [Header("ジャンプ長押し時の持続可能時間")]
        public float m_jumpDelayTimeMax = 0.5f;

        [Header("最大落下速度")]
        public float m_fallSpeedMin = -10.0f;

        [Header("地面判定")]
        public LayerMask m_groundLayerMask = 0;

        [Header("地面判定開始位置"), SerializeField, Tooltip("地面判定を行うTransformを指定してください。")]
        public Vector2 m_groundCheckPosition = Vector2.zero;

        [Header("地面判定距離")]
        public float m_groundCheckDistance = 0.1f;

        [Header("地面判定幅")]
        public float m_groundCheckWidth = 1.9f;
    }

    [System.Serializable]
    public class PlayerAudioData
    {
        [Header("歩行時のSE")]
        public AudioComponent m_moveSe;

        [Header("ダッシュ時のSE")]
        public AudioComponent m_speedMoveSe;

        [Header("ジャンプ時のSE")]
        public AudioComponent m_jumpSe;

        [Header("着地時のSE")]
        public AudioComponent m_groundSe;
    }
}

namespace Action3d
{
    [System.Serializable]
    public class PlayerMoveData
    {
        [Header("移動速度")]
        public float m_speed = 5.0f;

        [Header("ジャンプ初速")]
        public float m_initialJumpForce = 5.0f;

        [Header("ジャンプ長押し時の継続的上昇速度")]
        public float m_sustainedJumpForce = 2.0f;

        [Header("ジャンプ長押し時の持続可能時間")]
        public float m_jumpDelayTimeMax = 0.5f;

        [Header("最大落下速度")]
        public float m_fallSpeedMin = -10.0f;

        [Header("地面判定")]
        public LayerMask m_groundLayerMask = 0;

        [Header("地面判定開始位置"), SerializeField, Tooltip("地面判定を行うTransformを指定してください。")]
        public Vector3 m_groundCheckPosition = Vector3.zero;

        [Header("地面判定距離")]
        public float m_groundCheckDistance = 0.1f;

        [Header("地面判定幅")]
        public float m_groundCheckWidth = 1.9f;
    }

    [System.Serializable]
    public class PlayerAudioData
    {
        [Header("歩行時のSE")]
        public AudioComponent m_moveSe;

        [Header("ダッシュ時のSE")]
        public AudioComponent m_speedMoveSe;

        [Header("ジャンプ時のSE")]
        public AudioComponent m_jumpSe;

        [Header("着地時のSE")]
        public AudioComponent m_groundSe;
    }
}