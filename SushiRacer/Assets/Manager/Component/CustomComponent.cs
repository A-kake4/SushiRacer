#if UNITY_EDITOR
using UnityEditor;
[CustomEditor( typeof( BaseComponent<CustomItem, CustomDataScriptableObject> ), true )]
public class CustomComponentEditor : BaseComponentEditor
{

}
#endif
[System.Serializable]
public class CustomItem : BaseItem
{
    // 必要に応じて追加のプロパティやメソッドを定義可能
    public int customValue; // 例: カスタム値
}

//[CreateAssetMenu( fileName = "New Custom Data", menuName = "Custom/Custom Data" )]
public class CustomDataScriptableObject : BaseDataScriptableObject<CustomItem>
{
    // 必要に応じて追加のプロパティやメソッドを定義可能
}

public class CustomComponent : BaseComponent<CustomItem, CustomDataScriptableObject>
{

}
