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

        // target �� SplineTransformUtility �ɃL���X�g
        SplineTransformUtility utility = target as SplineTransformUtility;
        if (utility == null)
        {
            EditorGUILayout.HelpBox( "�Ώۂ�SplineTransformUtility��������܂���B", MessageType.Warning );
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

        // Uniformize Spline Y �p�t�B�[���h�� SerializedProperty ���g���ĕ\��
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
    /// �I�u�W�F�N�g�Ƃ��̊֘A�I�u�W�F�N�g�ɑ΂��� Undo ����̋��ʏ������s���܂��B
    /// SplineTransformUtility �̏ꍇ�A���� GameObject ����擾�\�� SplineContainer ���L�^�ΏۂƂ��܂��B
    /// </summary>
    /// <param name="obj">����Ώۂ̃I�u�W�F�N�g</param>
    /// <param name="undoName">Undo ����̖��O</param>
    /// <param name="action">���s����A�N�V����</param>
    private void PerformUndoOperation( UnityEngine.Object obj, string undoName, Action action )
    {
        // �ύX�����\���̂���I�u�W�F�N�g���܂Ƃ߂�
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
