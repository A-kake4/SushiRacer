#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.Splines;

[CustomEditor( typeof( SplineWall ) )]
public class SplineWallEditor : Editor
{
    private SerializedProperty expandFactorProp;
    // �ǉ�: uniformY �v���p�e�B
    private SerializedProperty uniformYProp;

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        SplineContainer.SplineAdded += OnContainerSplineSetModified;
        SplineContainer.SplineRemoved += OnContainerSplineSetModified;

        expandFactorProp = serializedObject.FindProperty( "expandFactor" );
        uniformYProp = serializedObject.FindProperty( "uniformY" );
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
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            if (target is SplineWall)
            {
                ( target as SplineWall )?.Rebuild();
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button( "�X�v���C�������b�V���̌`��ɍX�V" ))
        {
            Undo.RecordObject( target, "Update Spline to Mesh Shape" );
            ( target as SplineWall )?.GenerateSplineFromMesh();
        }

        // �ǉ��@�\�F�X�v���C���̉�]�����Z�b�g
        EditorGUILayout.Space();
        EditorGUILayout.LabelField( "�ǉ��@�\", EditorStyles.boldLabel );
        if (GUILayout.Button( "�X�v���C���̉�]���Z�b�g" ))
        {
            Undo.RecordObject( target, "Reset Spline Rotation" );
            ( target as SplineWall )?.ResetSplineRotation();
        }

        // �ǉ��@�\�F�X�v���C���� Y ���W���ψꉻ
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField( uniformYProp, new GUIContent( "�ψꉻY�l" ) );
        if (GUILayout.Button( "�X�v���C����Y���W�ψꉻ" ))
        {
            Undo.RecordObject( target, "Uniformize Spline Y" );
            ( target as SplineWall )?.UniformizeSplineY();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button( "���b�V���̖ʂ𔽓]" ))
        {
            Undo.RecordObject( target, "Reverse Mesh Faces" );
            ( target as SplineWall )?.ReverseMeshFaces();
        }

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

    private void OnSplineModified()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (target is SplineWall component)
        {
            component.Rebuild();
        }
    }
}
#endif // UNITY_EDITOR
