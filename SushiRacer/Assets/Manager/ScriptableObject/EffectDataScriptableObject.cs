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

        Debug.LogError( "�w�肳�ꂽ�G�t�F�N�gID��������܂���: " + effectId );
        return null;
    }
}

// �C���X�y�N�^�[��ŕ\�������
[System.Serializable]
public class EffectItem : BaseItem
{
    // �G�t�F�N�g�v���n�u
    [SerializeField]
    private GameObject m_effectPrefab;
    public GameObject EffectPrefab
    {
        get { return m_effectPrefab; }
    }

    // �G�t�F�N�g�̐�����
    [SerializeField, Range(1, 500)]
    private int m_effectNum;
    public int EffectNum
    {
        get { return m_effectNum; }
    }
}
