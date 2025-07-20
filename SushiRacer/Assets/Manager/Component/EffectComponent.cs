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
            Debug.LogError( "EffectDataScriptableObjectが設定されていません" );
            return null;
        }
        if (selectedItemNumber < 0 || selectedItemNumber >= dataSource.items.Length)
        {
            Debug.LogError( "選択されたインデックスが無効です" );
            return null;
        }

        // LINQを使用して、選択されたインデックスに基づいてアイテムを取得
        return EffectManager.Instance.PlayEffect( dataSource.items[selectedItemNumber].id, _position, _parent );
    }
}


