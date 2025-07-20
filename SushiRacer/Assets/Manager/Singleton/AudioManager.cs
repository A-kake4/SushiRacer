using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//----------------------------------------
// 作成日：2024/08/30 作成者：藤原宏貴
// 更新日：2025/07/11 更新者：藤原宏貴
//----------------------------------------
// 音楽の管理を行うクラス
// AudioMixerを使用してマスター、BGM、SEの音量を管理する
//----------------------------------------
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField]
    private AudioMixer m_audioMixer;

    private const string MASTER_VOLUME = "Master";
    private const string BGM_VOLUME = "BGM";
    private const string SE_VOLUME = "SE";

    /// <summary>
    /// マスター音量 -80dB ~ 20dB
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
    /// BGMの全体音量 -80dB ~ 20dB
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
    /// SEの全体音量 -80dB ~ 20dB
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
    /// 音量をAudioMixerに設定します。
    /// </summary>
    /// <param name="parameter">AudioMixerのパラメータ名</param>
    /// <param name="volume">-80dB ~ 20dBの音量</param>
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

    // シーン内のBGM
    private readonly List<AudioComponent> m_bgmList = new();

    /// <summary>
    /// AudioComponentを登録する関数
    /// </summary>
    public bool RegisterBgmList( AudioComponent _setAudio )
    {
        if ( m_bgmList.Contains( _setAudio ) )
        {
            return false;
        }

        m_bgmList.Add( _setAudio );
        // AudioMixerのBGMグループを設定
        _setAudio.AudioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups( "BGM" )[0];
        return true;
    }

    /// <summary>
    /// AudioComponentを削除する関数
    /// </summary>
    public bool RemoveBgmList( AudioComponent _setAudio )
    {
        return m_bgmList.Remove( _setAudio );
    }
}
