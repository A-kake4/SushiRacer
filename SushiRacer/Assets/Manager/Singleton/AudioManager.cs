using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//----------------------------------------
// �쐬���F2024/08/30 �쐬�ҁF�����G�M
// �X�V���F2025/07/11 �X�V�ҁF�����G�M
//----------------------------------------
// ���y�̊Ǘ����s���N���X
// AudioMixer���g�p���ă}�X�^�[�ABGM�ASE�̉��ʂ��Ǘ�����
//----------------------------------------
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioMixer m_audioMixer;

    private const string MASTER_VOLUME = "Master";
    private const string BGM_VOLUME = "BGM";
    private const string SE_VOLUME = "SE";

    /// <summary>
    /// �}�X�^�[���� -80dB ~ 20dB
    /// </summary>
    public float MasterVolume
    {
        get => GetVolume( MASTER_VOLUME );
        set
        {
            var volume = Mathf.Clamp( value, -80f, 20f );
            SetVolume( MASTER_VOLUME, volume );
        }
    }

    public AudioMixerGroup MasterAudioGroup
    {
        get => m_audioMixer.FindMatchingGroups( MASTER_VOLUME )[0];
    }

    /// <summary>
    /// BGM�̑S�̉��� -80dB ~ 20dB
    /// </summary>
    public float BgmVolume
    {
        get => GetVolume( BGM_VOLUME );
        set
        {
            var volume = Mathf.Clamp( value, -80f, 20f );
            SetVolume( BGM_VOLUME, volume );
        }
    }

    public AudioMixerGroup BgmAudioGroup
    {
        get => m_audioMixer.FindMatchingGroups( BGM_VOLUME )[0];
    }

    /// <summary>
    /// SE�̑S�̉��� -80dB ~ 20dB
    /// </summary>
    public float SeVolume
    {
        get => GetVolume( SE_VOLUME );
        set
        {
            var volume = Mathf.Clamp( value, -80f, 20f );
            SetVolume( SE_VOLUME, volume );
        }
    }

    public AudioMixerGroup SeAudioGroup
    {
        get => m_audioMixer.FindMatchingGroups( SE_VOLUME )[0];
    }

    protected override void AwakeSingleton()
    {
    }

    protected override void OnDestroySingleton()
    {
    }

    /// <summary>
    /// ���ʂ�AudioMixer�ɐݒ肵�܂��B
    /// </summary>
    /// <param name="parameter">AudioMixer�̃p�����[�^��</param>
    /// <param name="volume">-80dB ~ 20dB�̉���</param>
    private void SetVolume( string parameter, float volume )
    {
        m_audioMixer.SetFloat( parameter, volume );
    }

    /// <summary>
    /// 
    /// </summary>
    private float GetVolume( string parameter )
    {
        m_audioMixer.GetFloat( parameter, out float volume );
        return volume;
    }

    // �V�[������BGM
    private readonly List<AudioComponent> m_bgmList = new();

    /// <summary>
    /// AudioComponent��o�^����֐�
    /// </summary>
    public bool RegisterBgmList( AudioComponent _setAudio )
    {
        if ( m_bgmList.Contains( _setAudio ) )
        {
            return false;
        }

        m_bgmList.Add( _setAudio );
        // AudioMixer��BGM�O���[�v��ݒ�
        _setAudio.AudioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups( "BGM" )[0];
        return true;
    }

    /// <summary>
    /// AudioComponent���폜����֐�
    /// </summary>
    public bool RemoveBgmList( AudioComponent _setAudio )
    {
        return m_bgmList.Remove( _setAudio );
    }
}
