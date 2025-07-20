using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    [SerializeField]
    private EffectDataScriptableObject m_effectDatas;
    public EffectDataScriptableObject EffectDatas => m_effectDatas;

    private Dictionary<string, Queue<GameObject>> m_effectPool;
    private GameObject m_effectContainer;


    protected override void AwakeSingleton()
    {
        // �V���O���g���̃C���X�^���X�����ɑ��݂���ꍇ�́A�ēx�������Ȃ�
        m_effectContainer = new GameObject( "EffectContainer" );

        // dontDestroyOnLoad��true�̏ꍇ�A�V�[�����܂����ŃC���X�^���X��ێ�����
        if ( dontDestroyOnLoad )
        {
            DontDestroyOnLoad( m_effectContainer );
        }

        m_effectPool = new Dictionary<string, Queue<GameObject>>();

        foreach ( EffectItem effectData in m_effectDatas.items )
        {
            Queue<GameObject> effectQueue = new();
            m_effectPool.Add( effectData.id, effectQueue );

            for ( int i = 0; i < effectData.EffectNum; i++ )
            {
                GameObject effect = CreateEffect( effectData.id );
                if ( effect != null )
                {
                    effect.transform.SetParent( m_effectContainer.transform );
                    effect.SetActive( false );
                }
            }
        }
    }

    private GameObject CreateEffect( string _id )
    {
        EffectItem effectData = m_effectDatas.GetEffectItem( _id );
        if (effectData == null)
        {
           Debug.LogWarning( $"�G�t�F�N�g�f�[�^�����݂��܂���: ID = {_id}" );
            return null;
        }

        // �G�t�F�N�g�̃v���n�u���ݒ肳��Ă��Ȃ��ꍇ�̓G���[
        if ( effectData.EffectPrefab == null )
        {
            Debug.LogWarning( $"�G�t�F�N�g�̃v���n�u���ݒ肳��Ă��܂���: ID = {_id}" );
            return null;
        }

        GameObject effect = Instantiate( effectData.EffectPrefab );

        // �G�t�F�N�g��EffectController�R���|�[�l���g�������ꍇ�͕t�^����
        if (!effect.TryGetComponent<EffectController>( out var controller ))
        {
            controller = effect.AddComponent<EffectController>();
        }

        // �G�t�F�N�g��ID��ݒ�
        controller.EffectId = _id;
        controller.OnEffectDisabled += ( effectId, effectObject ) =>
        {
            StopEffect( effectId, effectObject );
        };

        return effect;
    }

    public GameObject PlayEffect( string id, Vector3 position = default, Transform parent = null )
    {
        if (!m_effectPool.TryGetValue( id, out Queue<GameObject> effectQueue ) || effectQueue.Count == 0)
        {
            LogError( $"�G�t�F�N�g�̎擾�Ɏ��s���܂���: ID = {id}" );
            return null;
        }

        // �G�t�F�N�g���擾
        GameObject effect = effectQueue.Dequeue();

        if(effect.activeInHierarchy)
        {
            Debug.Log( "�Đ����̃G�t�F�N�g : ");
        }


        effect.transform.position = position;
        effect.transform.SetParent( parent != null ? parent : m_effectContainer.transform );
        effect.SetActive( true );

        return effect;
    }

    public void StopEffect( string _id, GameObject _effectObject )
    {
        if (m_effectPool.TryGetValue( _id, out Queue<GameObject> effectQueue ))
        {
            _effectObject.SetActive( false );
            _effectObject.transform.SetParent( m_effectContainer.transform );

            // �G�t�F�N�g���v�[���ɖ߂�
            effectQueue.Enqueue( _effectObject );

            // �e���ς���Ă���ꍇ�͌��̐e�ɖ߂�
            if (_effectObject.transform.parent != m_effectContainer.transform)
            {
                _effectObject.transform.SetParent( m_effectContainer.transform );
            }
        }
        else
        {
            LogError( $"�G�t�F�N�g�̒�~�Ɏ��s���܂���: ID = {_id}" );
        }
    }

    private void LogError( string message )
    {
        Debug.LogError( $"[EffectManager] {message}" );
    }

    protected override void OnDestroySingleton()
    {
    }
}
