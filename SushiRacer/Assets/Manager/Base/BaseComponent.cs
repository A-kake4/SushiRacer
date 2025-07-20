using UnityEngine;

public abstract class BaseComponent<TItem, TData> : MonoBehaviour
    where TItem : BaseItem
    where TData : BaseDataScriptableObject<TItem>
{
    [Tooltip( "�f�[�^�\�[�X�ƂȂ�ScriptableObject" )]
    public TData dataSource;

    // �ҏW�ł��Ȃ��l��
    [Tooltip( "�I�����ꂽ�A�C�e���̃C���f�b�N�X" ), ReadOnly]
    public int selectedItemNumber;

    [Tooltip( "�I�����ꂽ�A�C�e����ID" ), ReadOnly]
    public string selectedItemID;

    /// <summary>
    /// �f�[�^�\�[�X���L�����ǂ������m�F���܂��B
    /// </summary>
    private bool IsDataSourceValid => dataSource != null;

    /// <summary>
    /// ���ݑI������Ă���A�C�e�����擾���܂��B
    /// </summary>
    /// <returns>�I�����ꂽ�A�C�e���A�܂���null</returns>
    public TItem GetSelectedItem()
    {
        if (!IsDataSourceValid)
        {
            Debug.LogError( "[BaseComponent] dataSource���ݒ肳��Ă��܂���B" );
            return null;
        }

        var selectedItem = dataSource.GetItem( selectedItemNumber );
        if (selectedItem == null)
        {
            Debug.LogWarning( $"[BaseComponent] �C���f�b�N�X {selectedItemNumber} �ɑΉ�����A�C�e����������܂���ł����B" );
        }
        return selectedItem;
    }
}
