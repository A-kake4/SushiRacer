#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using UnityEditor.Splines;

[CustomEditor( typeof( SplineWall ) )]
public class SplineWallEditor : Editor
{
    // SerializedProperty を定義
    private SerializedProperty expandFactorProp;

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        SplineContainer.SplineAdded += OnContainerSplineSetModified;
        SplineContainer.SplineRemoved += OnContainerSplineSetModified;

        // SerializedProperty を取得
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
        // シリアライズされたオブジェクトの更新
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
        EditorGUILayout.PropertyField( expandFactorProp, new GUIContent( "スプライン更新" ) );
        if ( GUILayout.Button( "スプラインをメッシュの形状に更新" ) )
        {
            Undo.RecordObject( target, "Update Spline to Mesh Shape" );
            ( target as SplineWall )?.GenerateSplineFromMesh();
        }

        // ボタンの追加
        EditorGUILayout.Space();
        EditorGUILayout.LabelField( "追加機能", EditorStyles.boldLabel );

            if ( GUILayout.Button( "メッシュの面を反転" ) )
        {
            Undo.RecordObject( target, "Reverse Mesh Faces" );
            ( target as SplineWall )?.ReverseMeshFaces();
        }

        // ExpandFactor の入力フィールドを追加 (SerializedProperty を使用)
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField( expandFactorProp, new GUIContent( "拡大率" ) );
        if ( EditorGUI.EndChangeCheck() )
        {
            Undo.RecordObject( target, "Change Expand Factor" );
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty( target );
        }

        if ( GUILayout.Button( "メッシュの内側を拡大/縮小" ) )
        {
            Undo.RecordObject( target, "Expand Mesh Inner Side" );
            ( target as SplineWall )?.ExpandMeshInnerSide();
        }



        // シリアライズされたオブジェクトの適用
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
            // プレイモード中なら何もしない
            return;
        }

        // 本来は対象のSplineが編集されたときだけメッシュを再計算する方がいい
        if ( target is SplineWall component )
        {
            component.Rebuild();
        }
    }
}

#endif // UNITY_EDITOR