using UnityEngine;

[CreateAssetMenu( fileName = "EffectData", menuName = "Manager/EffectDatas", order = 0 )]
public class EffectDataScriptableObject : BaseDataScriptableObject<EffectItem>
{
    public EffectItem GetEffectItem( string effectId )
    {
        foreach (EffectItem effectData in items )
        {
            if ( effectData.id == effectId )
            {
                return effectData;
            }
        }

        Debug.LogError( "指定されたエフェクトIDが見つかりません: " + effectId );
        return null;
    }
}

// インスペクター上で表示される
[System.Serializable]
public class EffectItem : BaseItem
{
    // エフェクトプレハブ
    [SerializeField]
    private GameObject m_effectPrefab;
    public GameObject EffectPrefab
    {
        get { return m_effectPrefab; }
    }

    // エフェクトの生成数
    [SerializeField, Range(1, 500)]
    private int m_effectNum;
    public int EffectNum
    {
        get { return m_effectNum; }
    }
}
