using UnityEngine;

namespace Action2d
{
    [System.Serializable]
    public class PlayerMoveData
    {
        [Header("�ړ����x")]
        public float m_speed = 5.0f;

        [Header("�W�����v����")]
        public float m_initialJumpForce = 5.0f;

        [Header("�W�����v���������̌p���I�㏸���x")]
        public float m_sustainedJumpForce = 2.0f;

        [Header("�W�����v���������̎����\����")]
        public float m_jumpDelayTimeMax = 0.5f;

        [Header("�ő嗎�����x")]
        public float m_fallSpeedMin = -10.0f;

        [Header("�n�ʔ���")]
        public LayerMask m_groundLayerMask = 0;

        [Header("�n�ʔ���J�n�ʒu"), SerializeField, Tooltip("�n�ʔ�����s��Transform���w�肵�Ă��������B")]
        public Vector2 m_groundCheckPosition = Vector2.zero;

        [Header("�n�ʔ��苗��")]
        public float m_groundCheckDistance = 0.1f;

        [Header("�n�ʔ��蕝")]
        public float m_groundCheckWidth = 1.9f;
    }

    [System.Serializable]
    public class PlayerAudioData
    {
        [Header("���s����SE")]
        public AudioComponent m_moveSe;

        [Header("�_�b�V������SE")]
        public AudioComponent m_speedMoveSe;

        [Header("�W�����v����SE")]
        public AudioComponent m_jumpSe;

        [Header("���n����SE")]
        public AudioComponent m_groundSe;
    }
}

namespace Action3d
{
    [System.Serializable]
    public class PlayerMoveData
    {
        [Header("�ړ����x")]
        public float m_speed = 5.0f;

        [Header("�W�����v����")]
        public float m_initialJumpForce = 5.0f;

        [Header("�W�����v���������̌p���I�㏸���x")]
        public float m_sustainedJumpForce = 2.0f;

        [Header("�W�����v���������̎����\����")]
        public float m_jumpDelayTimeMax = 0.5f;

        [Header("�ő嗎�����x")]
        public float m_fallSpeedMin = -10.0f;

        [Header("�n�ʔ���")]
        public LayerMask m_groundLayerMask = 0;

        [Header("�n�ʔ���J�n�ʒu"), SerializeField, Tooltip("�n�ʔ�����s��Transform���w�肵�Ă��������B")]
        public Vector3 m_groundCheckPosition = Vector3.zero;

        [Header("�n�ʔ��苗��")]
        public float m_groundCheckDistance = 0.1f;

        [Header("�n�ʔ��蕝")]
        public float m_groundCheckWidth = 1.9f;
    }

    [System.Serializable]
    public class PlayerAudioData
    {
        [Header("���s����SE")]
        public AudioComponent m_moveSe;

        [Header("�_�b�V������SE")]
        public AudioComponent m_speedMoveSe;

        [Header("�W�����v����SE")]
        public AudioComponent m_jumpSe;

        [Header("���n����SE")]
        public AudioComponent m_groundSe;
    }
}