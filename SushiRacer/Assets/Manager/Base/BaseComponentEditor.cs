using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor( typeof( BaseComponent<,> ), true )]
public abstract class BaseComponentEditor : Editor
{
    private FieldInfo itemsField;
    private FieldInfo idField;

    public override void OnInspectorGUI()
    {
        // プロパティの更新を開始
        serializedObject.Update();

        // dataSource と selectedItemIndex プロパティを取得
        SerializedProperty dataSourceProp = serializedObject.FindProperty( "dataSource" );
        SerializedProperty selectedItemNumberProp = serializedObject.FindProperty( "selectedItemNumber" );
        SerializedProperty selectedItemNameProp = serializedObject.FindProperty( "selectedItemID" );

        if (dataSourceProp.objectReferenceValue != null)
        {
            var dataSource = dataSourceProp.objectReferenceValue;
            var dataSourceType = dataSource.GetType();

            // items フィールドをキャッシュして取得
            if (itemsField == null)
            {
                itemsField = dataSourceType.GetField( "items", BindingFlags.Public | BindingFlags.Instance );
            }

            if (itemsField != null)
            {
                var items = itemsField.GetValue( dataSource ) as System.Array;
                if (items != null && items.Length > 0)
                {
                    string[] options = GetItemOptions( items );
                    int currentIndex = Mathf.Clamp( selectedItemNumberProp.intValue, 0, items.Length - 1 );

                    int selectedNumber = EditorGUILayout.Popup( "Select Item", currentIndex, options );
                    if ( selectedNumber != currentIndex)
                    {
                        selectedItemNumberProp.intValue = selectedNumber;
                        selectedItemNameProp.stringValue = options[selectedNumber];
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox( "データソースにアイテムがありません。", MessageType.Warning );
                }
            }
            else
            {
                EditorGUILayout.HelpBox( "データソースに 'items' フィールドがありません。", MessageType.Warning );
            }
        }
        else
        {
            EditorGUILayout.HelpBox( "データソースが割り当てられていません。", MessageType.Warning );
        }

        // 変更を反映
        serializedObject.ApplyModifiedProperties();

        // デフォルトのインスペクターを描画
        DrawDefaultInspector();
    }

    /// <summary>
    /// アイテム配列から選択肢を生成します。
    /// </summary>
    /// <param name="items">アイテム配列</param>
    /// <returns>選択肢の文字列配列</returns>
    private string[] GetItemOptions( System.Array items )
    {
        string[] options = new string[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            var item = items.GetValue( i );
            if (item == null)
            {
                options[i] = "Unnamed Item";
                continue;
            }

            var itemType = item.GetType();

            // id フィールドをキャッシュして取得
            if (idField == null)
            {
                idField = itemType.GetField( "id", BindingFlags.Public | BindingFlags.Instance );

            }

            options[i] = idField != null ? (string)idField.GetValue( item ) : "Unnamed Item";
        }

        return options;
    }
}

#endif