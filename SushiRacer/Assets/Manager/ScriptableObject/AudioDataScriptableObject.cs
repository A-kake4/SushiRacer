using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu( fileName = "New Audio Data", menuName = "Manager/AudioData" )]
public class AudioDataScriptableObject : BaseDataScriptableObject<AudioItem>
{
    public AudioMixerGroup audioGroup; // AudioMixer�̃O���[�v
}

[System.Serializable]
public class AudioItem : BaseItem
{
    // �I�[�f�B�I�N���b�v
    [SerializeField]
    private AudioClip m_audioClip;
    public AudioClip AudioClip => m_audioClip;

    // �I�[�f�B�I�̉���
    [SerializeField, Range( 0.0f, 1.0f )]
    private float m_volume = 1.0f;
    public float Volume => m_volume;

    // �I�[�f�B�I�̃s�b�`
    [SerializeField, Range( -3.0f, 3.0f )]
    private float m_pitch = 1.0f;
    public float Pitch => m_pitch;

    // �I�[�f�B�I�̃��[�v
    [SerializeField]
    private bool m_loop = false;
    public bool IsLoop => m_loop;

    [SerializeField, Range( 0.0f, 1.0f )]
    private float m_spatialBlend = 1.0f;
    public float SpatialBlend => m_spatialBlend;
}