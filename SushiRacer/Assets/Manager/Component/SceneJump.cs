using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField]
    private SceneAsset sceneAsset;
#endif
    [SerializeField]
    private string sceneName;

    public string SceneName => sceneName;
}

[CustomPropertyDrawer( typeof( SceneReference ) )]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        // SceneAsset と sceneName のシリアライズ済みプロパティを取得
        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

        EditorGUI.BeginProperty( position, label, property );
        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();
        // SceneAsset の ObjectField を描画
        SceneAsset sceneAsset = EditorGUI.ObjectField(fieldRect, label, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false) as SceneAsset;
        if ( EditorGUI.EndChangeCheck() )
        {
            sceneAssetProp.objectReferenceValue = sceneAsset;
            sceneNameProp.stringValue = sceneAsset != null ? sceneAsset.name : string.Empty;
        }
        EditorGUI.EndProperty();
    }
}

public class SceneJump : MonoBehaviour
{
    [SerializeField, Header("ターゲットシーン")]
    private SceneReference targetScene;

    public void JumpToTargetScene()
    {
        if ( !string.IsNullOrEmpty( targetScene.SceneName ) )
        {
            SceneManager.LoadScene( targetScene.SceneName );
        }
        else
        {
            Debug.LogError( "Invalid target scene specified." );
        }
    }
}