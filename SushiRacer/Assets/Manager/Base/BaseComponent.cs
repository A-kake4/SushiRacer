using UnityEngine;

public abstract class BaseComponent<TItem, TData> : MonoBehaviour
    where TItem : BaseItem
    where TData : BaseDataScriptableObject<TItem>
{
    [Tooltip( "データソースとなるScriptableObject" )]
    public TData dataSource;

    // 編集できない様に
    [Tooltip( "選択されたアイテムのインデックス" ), ReadOnly]
    public int selectedItemNumber;

    [Tooltip( "選択されたアイテムのID" ), ReadOnly]
    public string selectedItemID;

    /// <summary>
    /// データソースが有効かどうかを確認します。
    /// </summary>
    private bool IsDataSourceValid => dataSource != null;

    /// <summary>
    /// 現在選択されているアイテムを取得します。
    /// </summary>
    /// <returns>選択されたアイテム、またはnull</returns>
    public TItem GetSelectedItem()
    {
        if (!IsDataSourceValid)
        {
            Debug.LogError( "[BaseComponent] dataSourceが設定されていません。" );
            return null;
        }

        var selectedItem = dataSource.GetItem( selectedItemNumber );
        if (selectedItem == null)
        {
            Debug.LogWarning( $"[BaseComponent] インデックス {selectedItemNumber} に対応するアイテムが見つかりませんでした。" );
        }
        return selectedItem;
    }
}
