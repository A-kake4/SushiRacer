using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu( fileName = "New Audio Data", menuName = "Manager/AudioData" )]
public class AudioDataScriptableObject : BaseDataScriptableObject<AudioItem>
{
    public AudioMixerGroup audioGroup; // AudioMixerのグループ
}

[System.Serializable]
public class AudioItem : BaseItem
{
    // オーディオクリップ
    [SerializeField]
    private AudioClip m_audioClip;
    public AudioClip AudioClip => m_audioClip;

    // オーディオの音量
    [SerializeField, Range( 0.0f, 1.0f )]
    private float m_volume = 1.0f;
    public float Volume => m_volume;

    // オーディオのピッチ
    [SerializeField, Range( -3.0f, 3.0f )]
    private float m_pitch = 1.0f;
    public float Pitch => m_pitch;

    // オーディオのループ
    [SerializeField]
    private bool m_loop = false;
    public bool IsLoop => m_loop;

    [SerializeField, Range( 0.0f, 1.0f )]
    private float m_spatialBlend = 1.0f;
    public float SpatialBlend => m_spatialBlend;
}