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
            // 現在のアクションマップ名を取得
            List<string> actionMapNames = inputManager.InputActionAsset.actionMaps.Select(m => m.name).ToList();

            // 現在選択されているゲームモード
            string currentMode = nowGameModeProp.stringValue;

            // ドロップダウンのインデックスを取得
            int selectedIndex = actionMapNames.IndexOf(currentMode);
            if ( selectedIndex == -1 )
            {
                selectedIndex = 0; // デフォルトは最初のアクションマップ
                nowGameModeProp.stringValue = actionMapNames.Count > 0 ? actionMapNames[0] : "";
            }

            // ドロップダウンを表示
            selectedIndex = EditorGUILayout.Popup( "現在のゲームモード", selectedIndex, actionMapNames.ToArray() );

            // 選択されたモードを取得
            if ( actionMapNames.Count > 0 )
            {
                string selectedMode = actionMapNames[selectedIndex];
                if ( selectedMode != currentMode )
                {
                    nowGameModeProp.stringValue = selectedMode;
                }
            }

            // ゲームモードの説明
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox( "現在のゲームモードを選択してください。選択したモード名は InputManager から参照できます。", MessageType.Info );
        }
        else
        {
            EditorGUILayout.HelpBox( "InputActionAsset をインスペクターで設定してください。", MessageType.Error );
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
