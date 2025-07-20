using UnityEngine;
using System.Linq;

public abstract class BaseDataScriptableObject<T> : ScriptableObject where T : BaseItem
{
    [Tooltip( "BaseItem型の配列" )]
    public T[] items;

    /// <summary>
    /// IDからアイテムを取得します。
    /// </summary>
    /// <param name="itemId">検索するアイテムのID</param>
    /// <returns>一致するアイテム、またはnull</returns>
    public T GetItem( string itemId )
    {
        if ( string.IsNullOrEmpty( itemId ) )
        {
            Debug.LogWarning( "[BaseDataScriptableObject] itemIdがnullまたは空です。" );
            return null;
        }

        // LINQを使用して検索を簡略化
        var item = items.FirstOrDefault( item => item.id == itemId );
        if ( item == null )
        {
            Debug.LogWarning( $"[BaseDataScriptableObject] ID '{itemId}' に一致するアイテムが見つかりませんでした。" );
        }
        return item;
    }

    /// <summary>
    /// インデックスからアイテムを取得します。
    /// </summary>
    /// <param name="index">取得するアイテムのインデックス</param>
    /// <returns>一致するアイテム、またはnull</returns>
    public T GetItem( int index )
    {
        if ( index < 0 || index >= items.Length )
        {
            Debug.LogWarning( $"[BaseDataScriptableObject] インデックス {index} は範囲外です (0 ～ {items.Length - 1})。" );
            return null;
        }

        return items[index];
    }
}