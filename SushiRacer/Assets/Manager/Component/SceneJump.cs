using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;

#endif
//[System.Serializable]

//public class SceneReference
//{
//#if UNITY_EDITOR

//    [SerializeField]
//    private SceneAsset sceneAsset;
//#endif

//    [SerializeField]
//    private string sceneName;

//    public string SceneName => sceneName;
//}
//#if UNITY_EDITOR
//[CustomPropertyDrawer( typeof( SceneReference ) )]
//public class SceneReferencePropertyDrawer : PropertyDrawer
//{
//    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
//    {
//        // SceneAsset �� sceneName �̃V���A���C�Y�ς݃v���p�e�B���擾
//        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
//        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

//        EditorGUI.BeginProperty( position, label, property );
//        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

//        EditorGUI.BeginChangeCheck();
//        // SceneAsset �� ObjectField ��`��
//        SceneAsset sceneAsset = EditorGUI.ObjectField(fieldRect, label, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false) as SceneAsset;
//        if ( EditorGUI.EndChangeCheck() )
//        {
//            sceneAssetProp.objectReferenceValue = sceneAsset;
//            sceneNameProp.stringValue = sceneAsset != null ? sceneAsset.name : string.Empty;
//        }
//        EditorGUI.EndProperty();
//    }
//}
//#endif

public class SceneJump : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField, Header("�^�[�Q�b�g�V�[��")]
    private SceneAsset targetScene;

    private void OnValidate()
    {
        sceneName = targetScene.name;
    }
#endif

    [ReadOnly,SerializeField]
    private string sceneName;

    public void JumpToTargetScene()
    {
        if ( sceneName != "" )
        {
            SceneManager.LoadScene( sceneName );
        }
        else
        {
            Debug.LogError( "Invalid target scene specified." );
        }
    }

    //�Q�[���I��
    public void EndGame()
    {
        //Esc�������ꂽ��

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }
}