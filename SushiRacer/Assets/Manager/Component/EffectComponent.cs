using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( BaseComponent<EffectItem, EffectDataScriptableObject> ), true )]
public class EffectComponentEditor : BaseComponentEditor
{

}
#endif

public class EffectComponent : BaseComponent<EffectItem, EffectDataScriptableObject>
{
    public GameObject PlayEffect(Vector3 _position = default, Transform _parent = null )
    {
        if (dataSource == null)
        {
            Debug.LogError( "EffectDataScriptableObject���ݒ肳��Ă��܂���" );
            return null;
        }
        if (selectedItemNumber < 0 || selectedItemNumber >= dataSource.items.Length)
        {
            Debug.LogError( "�I�����ꂽ�C���f�b�N�X�������ł�" );
            return null;
        }

        // LINQ���g�p���āA�I�����ꂽ�C���f�b�N�X�Ɋ�Â��ăA�C�e�����擾
        return EffectManager.Instance.PlayEffect( dataSource.items[selectedItemNumber].id, _position, _parent );
    }
}


