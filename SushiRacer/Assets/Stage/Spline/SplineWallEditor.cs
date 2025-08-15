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
    // �ǉ�: reverseFaces �v���p�e�B�i�{���ȊO�̋@�\�̏ꍇ�͂��̂܂܎c���܂��j
    private SerializedProperty reverseFacesProp;

    void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        SplineContainer.SplineAdded += OnContainerSplineSetModified;
        SplineContainer.SplineRemoved += OnContainerSplineSetModified;

        expandFactorProp = serializedObject.FindProperty( "expandFactor" );
        uniformYProp = serializedObject.FindProperty( "uniformY" );
        reverseFacesProp = serializedObject.FindProperty( "reverseFaces" );
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
