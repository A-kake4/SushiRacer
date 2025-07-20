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
    // �K�v�ɉ����Ēǉ��̃v���p�e�B�⃁�\�b�h���`�\
    public int customValue; // ��: �J�X�^���l
}

//[CreateAssetMenu( fileName = "New Custom Data", menuName = "Custom/Custom Data" )]
public class CustomDataScriptableObject : BaseDataScriptableObject<CustomItem>
{
    // �K�v�ɉ����Ēǉ��̃v���p�e�B�⃁�\�b�h���`�\
}

public class CustomComponent : BaseComponent<CustomItem, CustomDataScriptableObject>
{

}
