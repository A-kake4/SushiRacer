#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

[CustomEditor( typeof( InputManager ) )]
public class InputManagerEditor : Editor
{
    // SerializedProperty for nowGameMode
    SerializedProperty nowGameModeProp;

    private void OnEnable()
    {
        nowGameModeProp = serializedObject.FindProperty( "nowGameMode" );
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        InputManager inputManager = (InputManager)target;

        if ( inputManager.InputActionAsset != null )
        {
            // ���݂̃A�N�V�����}�b�v�����擾
            List<string> actionMapNames = inputManager.InputActionAsset.actionMaps.Select(m => m.name).ToList();

            // ���ݑI������Ă���Q�[�����[�h
            string currentMode = nowGameModeProp.stringValue;

            // �h���b�v�_�E���̃C���f�b�N�X���擾
            int selectedIndex = actionMapNames.IndexOf(currentMode);
            if ( selectedIndex == -1 )
            {
                selectedIndex = 0; // �f�t�H���g�͍ŏ��̃A�N�V�����}�b�v
                nowGameModeProp.stringValue = actionMapNames.Count > 0 ? actionMapNames[0] : "";
            }

            // �h���b�v�_�E����\��
            selectedIndex = EditorGUILayout.Popup( "���݂̃Q�[�����[�h", selectedIndex, actionMapNames.ToArray() );

            // �I�����ꂽ���[�h���擾
            if ( actionMapNames.Count > 0 )
            {
                string selectedMode = actionMapNames[selectedIndex];
                if ( selectedMode != currentMode )
                {
                    nowGameModeProp.stringValue = selectedMode;
                }
            }

            // �Q�[�����[�h�̐���
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox( "���݂̃Q�[�����[�h��I�����Ă��������B�I���������[�h���� InputManager ����Q�Ƃł��܂��B", MessageType.Info );
        }
        else
        {
            EditorGUILayout.HelpBox( "InputActionAsset ���C���X�y�N�^�[�Őݒ肵�Ă��������B", MessageType.Error );
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
