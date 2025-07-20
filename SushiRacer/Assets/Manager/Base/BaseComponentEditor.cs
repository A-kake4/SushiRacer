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
        // �v���p�e�B�̍X�V���J�n
        serializedObject.Update();

        // dataSource �� selectedItemIndex �v���p�e�B���擾
        SerializedProperty dataSourceProp = serializedObject.FindProperty( "dataSource" );
        SerializedProperty selectedItemNumberProp = serializedObject.FindProperty( "selectedItemNumber" );
        SerializedProperty selectedItemNameProp = serializedObject.FindProperty( "selectedItemID" );

        if (dataSourceProp.objectReferenceValue != null)
        {
            var dataSource = dataSourceProp.objectReferenceValue;
            var dataSourceType = dataSource.GetType();

            // items �t�B�[���h���L���b�V�����Ď擾
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
                    EditorGUILayout.HelpBox( "�f�[�^�\�[�X�ɃA�C�e��������܂���B", MessageType.Warning );
                }
            }
            else
            {
                EditorGUILayout.HelpBox( "�f�[�^�\�[�X�� 'items' �t�B�[���h������܂���B", MessageType.Warning );
            }
        }
        else
        {
            EditorGUILayout.HelpBox( "�f�[�^�\�[�X�����蓖�Ă��Ă��܂���B", MessageType.Warning );
        }

        // �ύX�𔽉f
        serializedObject.ApplyModifiedProperties();

        // �f�t�H���g�̃C���X�y�N�^�[��`��
        DrawDefaultInspector();
    }

    /// <summary>
    /// �A�C�e���z�񂩂�I�����𐶐����܂��B
    /// </summary>
    /// <param name="items">�A�C�e���z��</param>
    /// <returns>�I�����̕�����z��</returns>
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

            // id �t�B�[���h���L���b�V�����Ď擾
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