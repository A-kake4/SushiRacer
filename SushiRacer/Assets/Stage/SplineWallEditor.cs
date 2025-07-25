#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.Splines;

[CustomEditor( typeof( SplineWall ) )]
public class SplineWallEditor : Editor
{
    private SerializedProperty expandFactorProp;
    // 追加: uniformY プロパティ
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
        if (GUILayout.Button( "スプラインをメッシュの形状に更新" ))
        {
            Undo.RecordObject( target, "Update Spline to Mesh Shape" );
            ( target as SplineWall )?.GenerateSplineFromMesh();
        }

        // 追加機能：スプラインの回転をリセット
        EditorGUILayout.Space();
        EditorGUILayout.LabelField( "追加機能", EditorStyles.boldLabel );
        if (GUILayout.Button( "スプラインの回転リセット" ))
        {
            Undo.RecordObject( target, "Reset Spline Rotation" );
            ( target as SplineWall )?.ResetSplineRotation();
        }

        // 追加機能：スプラインの Y 座標を均一化
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField( uniformYProp, new GUIContent( "均一化Y値" ) );
        if (GUILayout.Button( "スプラインのY座標均一化" ))
        {
            Undo.RecordObject( target, "Uniformize Spline Y" );
            ( target as SplineWall )?.UniformizeSplineY();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button( "メッシュの面を反転" ))
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
