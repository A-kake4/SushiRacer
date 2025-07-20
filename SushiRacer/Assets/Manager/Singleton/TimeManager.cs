using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//----------------------------------------
// �������F2025/04/16
// �X�V���F2025/04/16
// �X�V�ҁF����
//----------------------------------------
// timeScale��ύX���邱�ƂŁA���Ԃ̗���𐧌䂷��N���X
// �O���̃N���X����Ăяo�����ƂŁA���̃N���X���̎��Ԃ̐��l���L�^
// ���̃N���X�Ƃ̎��Ԃ̗���̏���������Ă��Ă�
// ���ꂼ��̃N���X�Ŏ��Ԃ̗���𐧌�ł���悤�ɂ���
//-----------------------------------------
// �g����
// �����Ƃ���MonoBehaviour��n�����ƂŁA�ǂ̃N���X����̌Ăяo��������肷��
//
// 1. �Ăяo���R���|�[�l���g�Ƒ��x�������ɂ��āA���Ԃ̑�����ύX����
// ��jTimeManager.Instance.SetTimeScale(this, 0.5f);
//
// 2. �ύX�ɐ������Ԃ�������ꍇ
//    SetFixedTimeLimit ��
//    SetUnfixedTimeLimit �Ő������Ԃ�ݒ肷��
// ��j TimeManager.Instance.SetFixedTimeLimit(this, 5.0f);
// ���� TimeManager.Instance.SetUnfixedTimeLimit(this, 5.0f);
//
// 3. �蓮�ł����ɖ߂���
// ��jTimeManager.Instance.RemoveTimeScale(this);
//
// 4. �V�[����ς��鎞�͏���������
// ��jTimeManager.Instance.InitTimeScale();

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    /// <summary>
    /// ���ԃX�P�[�����L�^���邽�߂̎���
    /// </summary>
    private readonly Dictionary<int, float> timeScaleList = new();

    /// <summary>
    /// �Q�[�������Ԃ̌o�߂ŉ������邽�߂̎���
    /// </summary>
    private readonly Dictionary<int, float> fixedTimeLimitList = new();

    /// <summary>
    /// �����Ԃł̌o�߂ŉ������邽�߂̎���
    /// </summary>
    private readonly Dictionary<int, float> unfixedTimeLimitList = new();

    // �f�o�b�O����inspector��ŕ\�L���邽�߂̕ϐ�
    [SerializeField, ReadOnly]
    private float totalTimeScale = 1.0f;

    /// <summary>
    /// ������
    /// </summary>
    protected override void AwakeSingleton()
    {
        InitTimeScale();
    }

    /// <summary>
    /// �I�u�W�F�N�g�̎��ԃX�P�[����ݒ�
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    /// <param name="timeScale">�ݒ肷��X�P�[��</param>
    public void SetTimeScale( MonoBehaviour mono, float timeScale )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetTimeScale�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return;
        }

        int monoId = mono.GetInstanceID();
        timeScaleList[monoId] = Mathf.Clamp( timeScale, 0.0f, 10.0f ); // �X�P�[���l���N�����v
        UpdateTimeScale();
    }

    /// <summary>
    /// �Œ莞�Ԑ�����ݒ�
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    /// <param name="timeLimit">�ݒ肷�鎞�Ԑ���</param>
    public void SetFixedTimeLimit( MonoBehaviour mono, float timeLimit )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetFixedTimeLimit�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return;
        }

        int monoId = mono.GetInstanceID();
        fixedTimeLimitList[monoId] = Mathf.Max( timeLimit, 0.0f ); // ���Ԑ����𐳂̒l��
    }

    /// <summary>
    /// ��Œ莞�Ԑ�����ݒ�
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    /// <param name="timeLimit">�ݒ肷�鎞�Ԑ���</param>
    public void SetUnfixedTimeLimit( MonoBehaviour mono, float timeLimit )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetUnfixedTimeLimit�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return;
        }

        int monoId = mono.GetInstanceID();
        unfixedTimeLimitList[monoId] = Mathf.Max( timeLimit, 0.0f ); // ���Ԑ����𐳂̒l��
    }

    /// <summary>
    /// �L�^���Ă��������ԃX�P�[�����폜
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    public void RemoveTimeScale( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "RemoveTimeScale�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return;
        }

        RemoveTimeScale( mono.GetInstanceID(), true );
    }

    public void RemoveTimeScale( int monoId, bool showWarning = false )
    {
        bool removed = timeScaleList.Remove(monoId);
        bool fixedRemoved = fixedTimeLimitList.Remove(monoId);
        bool unfixedRemoved = unfixedTimeLimitList.Remove(monoId);

        if ( showWarning && !removed && !fixedRemoved && !unfixedRemoved )
        {
            Debug.LogWarning( $"�w�肳�ꂽ�I�u�W�F�N�g (ID: {monoId}) �̎��ԃX�P�[����������܂���B" );
        }
        UpdateTimeScale();
    }

    /// <summary>
    /// �I�u�W�F�N�g���w�肵�Ă��鎞�ԃX�P�[�����擾
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    public float GetTimeScale( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "GetTimeScale�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return 1.0f;
        }

        return timeScaleList.TryGetValue( mono.GetInstanceID(), out float timeScale ) ? timeScale : 1.0f;
    }

    /// <summary>
    /// �I�u�W�F�N�g���w�肵�Ă��鎞�Ԑ������擾
    /// </summary>
    /// <param name="mono">�Ăяo�����̃R���|�[�l���g(this)</param>
    public float GetTimeLimit( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "GetTimeLimit�ɓn���ꂽMonoBehaviour��null�ł��B" );
            return 1.0f;
        }

        bool hasFixed = fixedTimeLimitList.TryGetValue(mono.GetInstanceID(), out float fixedLimit);
        bool hasUnfixed = unfixedTimeLimitList.TryGetValue(mono.GetInstanceID(), out float unfixedLimit);

        if ( hasFixed && hasUnfixed )
        {
            return Mathf.Min( fixedLimit, unfixedLimit );
        }
        else if ( hasFixed )
        {
            return fixedLimit;
        }
        else if ( hasUnfixed )
        {
            return unfixedLimit;
        }
        else
        {
            return 1.0f;
        }
    }

    /// <summary>
    /// �S�̂̎��ԃX�P�[�����v�Z
    /// </summary>
    public float TotalTimeScale
    {
        get
        {
            // �S�Ă̎��ԃX�P�[�����|�����킹�đ����I�Ȏ��ԃX�P�[�����v�Z
            float timeScale = 1.0f;
            foreach ( var scale in timeScaleList.Values )
            {
                timeScale *= scale;
            }

            return timeScale;
        }
    }


    /// <summary>
    /// ���ԃX�P�[�����X�V
    /// </summary>
    private void UpdateTimeScale()
    {
        Time.timeScale = Mathf.Clamp( TotalTimeScale, 0.0f, 10.0f ); // �X�P�[���l���N�����v
#if UNITY_EDITOR
        totalTimeScale = Time.timeScale;
#endif
    }

    /// <summary>
    /// LateUpdate�Ŏ��Ԑ���������
    /// </summary>
    private void LateUpdate()
    {
        ProcessTimeLimits( fixedTimeLimitList, Time.deltaTime );
        ProcessTimeLimits( unfixedTimeLimitList, Time.unscaledDeltaTime );
    }

    /// <summary>
    /// ���Ԑ���������
    /// </summary>
    private void ProcessTimeLimits( Dictionary<int, float> timeLimitList, float deltaTime )
    {
        if (timeLimitList.Count == 0) return;
        var keys = timeLimitList.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i];
            if (timeLimitList.TryGetValue(key, out float remainingTime))
            {
                remainingTime -= deltaTime;
                if (remainingTime <= 0f)
                {
                    RemoveTimeScale(key, false); // �x�����o���Ȃ�
                }
                else
                {
                    timeLimitList[key] = remainingTime;
                }
            }
        }
    }

    protected override void OnDestroySingleton()
    {
        // �K�v�ɉ����ă��\�[�X���
    }

    /// <summary>
    /// ���������Ɏ��ԃX�P�[�������Z�b�g
    /// </summary>
    public void InitTimeScale()
    {
        timeScaleList.Clear();
        fixedTimeLimitList.Clear();
        unfixedTimeLimitList.Clear();
        UpdateTimeScale();
    }
}
