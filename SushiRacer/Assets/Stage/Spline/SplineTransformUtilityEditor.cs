#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Splines;

[CustomEditor( typeof( SplineTransformUtility ) )]
public class SplineTransformUtilityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // target を SplineTransformUtility にキャスト
        SplineTransformUtility utility = target as SplineTransformUtility;
        if (utility == null)
        {
            EditorGUILayout.HelpBox( "対象のSplineTransformUtilityが見つかりません。", MessageType.Warning );
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField( "Spline Transform Utility", EditorStyles.boldLabel );
        EditorGUILayout.Space();

        if (GUILayout.Button( "Reset Spline Rotation" ))
        {
            PerformUndoOperation( utility, "Reset Spline Rotation", () =>
            {
                utility.ResetSplineRotation();
            } );
        }

        EditorGUILayout.Space();

        // Uniformize Spline Y 用フィールドは SerializedProperty を使って表示
        EditorGUILayout.LabelField( "Uniformize Spline Y", EditorStyles.boldLabel );
        SerializedProperty uniformYProp = serializedObject.FindProperty( "uniformY" );
        EditorGUILayout.PropertyField( uniformYProp, new GUIContent( "Uniform Y Value" ) );

        if (GUILayout.Button( "Uniformize Spline Y" ))
        {
            PerformUndoOperation( utility, "Uniformize Spline Y", () =>
            {
                utility.UniformizeSplineY();
            } );
        }

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// オブジェクトとその関連オブジェクトに対する Undo 操作の共通処理を行います。
    /// SplineTransformUtility の場合、同じ GameObject から取得可能な SplineContainer も記録対象とします。
    /// </summary>
    /// <param name="obj">操作対象のオブジェクト</param>
    /// <param name="undoName">Undo 操作の名前</param>
    /// <param name="action">実行するアクション</param>
    private void PerformUndoOperation( UnityEngine.Object obj, string undoName, Action action )
    {
        // 変更される可能性のあるオブジェクトをまとめる
        List<UnityEngine.Object> recordObjects = new List<UnityEngine.Object>
        {
            obj
        };

        if (obj is SplineTransformUtility utility)
        {
            SplineContainer container = utility.GetComponent<SplineContainer>();
            if (container != null)
            {
                recordObjects.Add( container );
            }
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        Undo.RecordObjects( recordObjects.ToArray(), undoName );
        action();
        foreach (var o in recordObjects)
        {
            EditorUtility.SetDirty( o );
        }
        Undo.CollapseUndoOperations( group );
    }
}

#endif
