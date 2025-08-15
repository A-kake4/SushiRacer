#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine.Splines;

[CustomEditor( typeof( Spline3DWall ) )]
public class Spline3DWallEditor : Editor
{
    private SerializedProperty reverseFacesProp;

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        SplineContainer.SplineAdded += OnContainerSplineSetModified;
        SplineContainer.SplineRemoved += OnContainerSplineSetModified;

        reverseFacesProp = serializedObject.FindProperty( "reverseFaces" );
    }

    private void OnDisable()
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
            if (target is Spline3DWall wall)
            {
                wall.Rebuild();
            }
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
            return;

        if (target is Spline3DWall wall)
        {
            wall.Rebuild();
        }
    }
}
#endif