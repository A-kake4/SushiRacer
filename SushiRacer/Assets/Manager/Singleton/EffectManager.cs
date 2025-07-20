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
        // シングルトンのインスタンスが既に存在する場合は、再度生成しない
        m_effectContainer = new GameObject( "EffectContainer" );

        // dontDestroyOnLoadがtrueの場合、シーンをまたいでインスタンスを保持する
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
           Debug.LogWarning( $"エフェクトデータが存在しません: ID = {_id}" );
            return null;
        }

        // エフェクトのプレハブが設定されていない場合はエラー
        if ( effectData.EffectPrefab == null )
        {
            Debug.LogWarning( $"エフェクトのプレハブが設定されていません: ID = {_id}" );
            return null;
        }

        GameObject effect = Instantiate( effectData.EffectPrefab );

        // エフェクトにEffectControllerコンポーネントが無い場合は付与する
        if (!effect.TryGetComponent<EffectController>( out var controller ))
        {
            controller = effect.AddComponent<EffectController>();
        }

        // エフェクトのIDを設定
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
            LogError( $"エフェクトの取得に失敗しました: ID = {id}" );
            return null;
        }

        // エフェクトを取得
        GameObject effect = effectQueue.Dequeue();

        if(effect.activeInHierarchy)
        {
            Debug.Log( "再生中のエフェクト : ");
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

            // エフェクトをプールに戻す
            effectQueue.Enqueue( _effectObject );

            // 親が変わっている場合は元の親に戻す
            if (_effectObject.transform.parent != m_effectContainer.transform)
            {
                _effectObject.transform.SetParent( m_effectContainer.transform );
            }
        }
        else
        {
            LogError( $"エフェクトの停止に失敗しました: ID = {_id}" );
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
