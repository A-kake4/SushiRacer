#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.Splines;

[CustomEditor( typeof( SplineWall ) )]
public class SplineWallEditor : Editor
{
    // SerializedProperty ���`
    private SerializedProperty expandFactorProp;

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        SplineContainer.SplineAdded += OnContainerSplineSetModified;
        SplineContainer.SplineRemoved += OnContainerSplineSetModified;

        // SerializedProperty ���擾
        expandFactorProp = serializedObject.FindProperty( "expandFactor" );
    }

    void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified -= OnSplineModified;
        SplineContainer.SplineAdded -= OnContainerSplineSetModified;
        SplineContainer.SplineRemoved -= OnContainerSplineSetModified;
    }

    public override void OnInspectorGUI()
    {
        // �V���A���C�Y���ꂽ�I�u�W�F�N�g�̍X�V
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if ( EditorGUI.EndChangeCheck() )
        {
            if ( target is SplineWall )
            {
                ( target as SplineWall )?.Rebuild();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField( expandFactorProp, new GUIContent( "�X�v���C���X�V" ) );
        if ( GUILayout.Button( "�X�v���C�������b�V���̌`��ɍX�V" ) )
        {
            Undo.RecordObject( target, "Update Spline to Mesh Shape" );
            ( target as SplineWall )?.GenerateSplineFromMesh();
        }

        // �{�^���̒ǉ�
        EditorGUILayout.Space();
        EditorGUILayout.LabelField( "�ǉ��@�\", EditorStyles.boldLabel );

            if ( GUILayout.Button( "���b�V���̖ʂ𔽓]" ) )
        {
            Undo.RecordObject( target, "Reverse Mesh Faces" );
            ( target as SplineWall )?.ReverseMeshFaces();
        }

        // ExpandFactor �̓��̓t�B�[���h��ǉ� (SerializedProperty ���g�p)
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField( expandFactorProp, new GUIContent( "�g�嗦" ) );
        if ( EditorGUI.EndChangeCheck() )
        {
            Undo.RecordObject( target, "Change Expand Factor" );
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty( target );
        }

        if ( GUILayout.Button( "���b�V���̓������g��/�k��" ) )
        {
            Undo.RecordObject( target, "Expand Mesh Inner Side" );
            ( target as SplineWall )?.ExpandMeshInnerSide();
        }



        // �V���A���C�Y���ꂽ�I�u�W�F�N�g�̓K�p
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSplineChanged( Spline spline, int knotIndex, SplineModification modificationType )
    {
        OnSplineModified();
    }

    private void OnSplineModified( Spline spline )
    {
        OnSplineModified();
    }

    private void OnContainerSplineSetModified( SplineContainer container, int spline )
    {
        OnSplineModified();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnSplineModified()
    {
        if ( EditorApplication.isPlayingOrWillChangePlaymode )
        {
            // �v���C���[�h���Ȃ牽�����Ȃ�
            return;
        }

        // �{���͑Ώۂ�Spline���ҏW���ꂽ�Ƃ��������b�V�����Čv�Z�����������
        if ( target is SplineWall component )
        {
            component.Rebuild();
        }
    }
}

#endif // UNITY_EDITOR