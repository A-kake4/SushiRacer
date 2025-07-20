using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BaseComponent<AudioItem, AudioDataScriptableObject> ), true )]
public class AudioComponentEditor : BaseComponentEditor { }
#endif

[RequireComponent( typeof( AudioSource ) )]
public class AudioComponent : BaseComponent<AudioItem, AudioDataScriptableObject>
{
    [SerializeField]
    private AudioSource m_audioSource;

    /// <summary>���ݍĐ������ǂ���</summary>
    public bool IsPlaying => m_audioSource != null && m_audioSource.isPlaying;
    public bool IsPlayOnAwake { get; set; } = false;

    /// <summary>���ݑI�𒆂�AudioItem</summary>
    public AudioItem CurrentAudioItem
    {
        get
        {
            if ( TryGetCurrentAudioItem( out var item ) )
                return item;
            return null;
        }
        set
        {
            if ( dataSource == null || dataSource.items == null )
                return;
            for ( int i = 0; i < dataSource.items.Length; i++ )
            {
                if ( dataSource.items[i] == value )
                {
                    selectedItemNumber = i;
                    ApplyAudioItemToSource();
                    return;
                }
            }
        }
    }

    /// <summary>AudioSource�̎Q��</summary>
    public AudioSource AudioSource => m_audioSource;

    /// <summary>�Đ��J�n���C�x���g</summary>
    public event Action OnPlay;
    /// <summary>�Đ���~���C�x���g</summary>
    public event Action OnStop;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if ( m_audioSource == null )
            m_audioSource = GetComponent<AudioSource>();
    }

    private void Reset()
    {
        m_audioSource = GetComponent<AudioSource>();
        if ( m_audioSource != null )
            m_audioSource.playOnAwake = false;
    }

    private void OnValidate()
    {
        ApplyAudioItemToSource();
    }

    private void Start()
    {
        ApplyAudioItemToSource();

        if ( IsPlayOnAwake )
        {
            m_audioSource?.Play();
            OnPlay?.Invoke();
        }
    }

    /// <summary>
    /// AudioItem�̐ݒ��AudioSource�ɔ��f
    /// </summary>
    public void ApplyAudioItemToSource()
    {
        if ( !TryGetCurrentAudioItem( out var audioItem ) || m_audioSource == null )
            return;

        m_audioSource.clip = audioItem.AudioClip;
        m_audioSource.volume = audioItem.Volume;
        m_audioSource.pitch = audioItem.Pitch;
        m_audioSource.loop = audioItem.IsLoop;
        m_audioSource.spatialBlend = audioItem.SpatialBlend;

        m_audioSource.outputAudioMixerGroup = dataSource.audioGroup;
    }

    /// <summary>
    /// �Đ�
    /// </summary>
    public void Play()
    {
        if ( m_audioSource == null )
        {
            Debug.LogWarning( "�G���[�FAudioSource������܂���" );
            return;
        }
        if ( !TryGetCurrentAudioItem( out var audioItem ) )
        {
            Debug.LogWarning( "�G���[�FAudioItem������܂���" );
            return;
        }

        m_audioSource.Play();
        OnPlay?.Invoke(); // ���S�ȌĂяo��
    }

    /// <summary>
    /// ��~
    /// </summary>
    public void Stop()
    {
        if ( m_audioSource != null && m_audioSource.isPlaying )
        {
            m_audioSource.Stop();
            OnStop?.Invoke();
        }
    }

    /// <summary>
    /// �Đ�/��~�g�O��
    /// </summary>
    public void Toggle()
    {
        if ( IsPlaying )
            Stop();
        else
            Play();
    }

    private void OnDestroy()
    {
        if ( fadeCoroutine != null )
            StopCoroutine( fadeCoroutine );
    }

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    public void FadeIn( float duration, float targetVolume = 1 )
    {
        if ( !TryGetCurrentAudioItem( out var audioItem ) )
            return;

        float toVolume = Mathf.Clamp01(targetVolume) * audioItem.Volume;

        if ( fadeCoroutine != null )
            StopCoroutine( fadeCoroutine );

        fadeCoroutine = StartCoroutine(
            FadeVolumeCoroutine(
                m_audioSource.volume,
                toVolume,
                duration
            )
        );
        if ( !m_audioSource.isPlaying )
        {
            m_audioSource.Play();
            OnPlay?.Invoke();
        }
    }



    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    public void FadeOut( float duration, float? endVolume = null )
    {
        float defaultEndVolume = Mathf.Clamp01(endVolume ?? 0f);

        if ( fadeCoroutine != null )
            StopCoroutine( fadeCoroutine );

        fadeCoroutine = StartCoroutine(
            FadeVolumeCoroutine( m_audioSource.volume, 
                                 defaultEndVolume, 
                                 duration, 
                                 stopOnEnd:defaultEndVolume == 0f ) 
            );
    }

    private IEnumerator FadeVolumeCoroutine( float from, float to, float duration, bool stopOnEnd = false )
    {
        if ( Mathf.Approximately( from, to ) )
        {
            yield break;
        }

        float elapsed = 0f;
        while ( elapsed < duration )
        {
            elapsed += Time.deltaTime;
            float newVolume = Mathf.Lerp(from, to, elapsed / duration);
            m_audioSource.volume = Mathf.Clamp01( newVolume );
            yield return null;
        }
        m_audioSource.volume = to;
        if ( stopOnEnd && to == 0f )
        {
            m_audioSource.Stop();
            OnStop?.Invoke();
        }
        fadeCoroutine = null;
    }


    /// <summary>
    /// ���݂�AudioItem�����S�Ɏ擾
    /// </summary>
    public bool TryGetCurrentAudioItem( out AudioItem audioItem )
    {
        audioItem = null;
        if ( dataSource == null || dataSource.items == null )
            return false;
        if ( selectedItemNumber < 0 || selectedItemNumber >= dataSource.items.Length )
            return false;
        audioItem = dataSource.items[selectedItemNumber];
        return audioItem != null;
    }
}
