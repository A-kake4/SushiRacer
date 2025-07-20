using UnityEngine;
using System.Linq;

public abstract class BaseDataScriptableObject<T> : ScriptableObject where T : BaseItem
{
    [Tooltip( "BaseItem�^�̔z��" )]
    public T[] items;

    /// <summary>
    /// ID����A�C�e�����擾���܂��B
    /// </summary>
    /// <param name="itemId">��������A�C�e����ID</param>
    /// <returns>��v����A�C�e���A�܂���null</returns>
    public T GetItem( string itemId )
    {
        if ( string.IsNullOrEmpty( itemId ) )
        {
            Debug.LogWarning( "[BaseDataScriptableObject] itemId��null�܂��͋�ł��B" );
            return null;
        }

        // LINQ���g�p���Č������ȗ���
        var item = items.FirstOrDefault( item => item.id == itemId );
        if ( item == null )
        {
            Debug.LogWarning( $"[BaseDataScriptableObject] ID '{itemId}' �Ɉ�v����A�C�e����������܂���ł����B" );
        }
        return item;
    }

    /// <summary>
    /// �C���f�b�N�X����A�C�e�����擾���܂��B
    /// </summary>
    /// <param name="index">�擾����A�C�e���̃C���f�b�N�X</param>
    /// <returns>��v����A�C�e���A�܂���null</returns>
    public T GetItem( int index )
    {
        if ( index < 0 || index >= items.Length )
        {
            Debug.LogWarning( $"[BaseDataScriptableObject] �C���f�b�N�X {index} �͔͈͊O�ł� (0 �` {items.Length - 1})�B" );
            return null;
        }

        return items[index];
    }
}