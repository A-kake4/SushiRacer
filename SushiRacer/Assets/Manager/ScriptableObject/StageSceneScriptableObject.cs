using UnityEngine;

[CreateAssetMenu( fileName = "New StageScene Data", menuName = "Manager/StageSceneData" )]
public class StageSceneScriptableObject : BaseDataScriptableObject<StageSceneItem>
{
    public StageSceneItem GetStageScene( int stageIndex )
    {
        if ( stageIndex < 0 || stageIndex >= items.Length )
        {
            Debug.LogWarning( $"[StageSceneScriptableObject] Stage ID {stageIndex} is out of range (0 - {items.Length - 1})." );
            return null;
        }
        // LINQ���g�p���āA�w�肳�ꂽ�X�e�[�WID�Ɉ�v����A�C�e��������
        var stageScene = items[stageIndex];
        if ( stageScene != null )
        {
            return stageScene;
        }

        // �w�肳�ꂽID�̃X�e�[�W��������Ȃ��ꍇ��null��Ԃ�
        Debug.LogError( $"[StageSceneScriptableObject] Stage ID {stageIndex} not found." );
        return null;
    }

    public StageSceneItem GetStageScene( string stageId )
    {
        // LINQ���g�p���āA�w�肳�ꂽ�X�e�[�WID�Ɉ�v����A�C�e��������
        var stageScene = System.Array.Find( items, item => item.id == stageId );
        if ( stageScene != null )
        {
            return stageScene;
        }
        // �w�肳�ꂽID�̃X�e�[�W��������Ȃ��ꍇ��null��Ԃ�
        Debug.LogError( $"[StageSceneScriptableObject] Stage ID {stageId} not found." );
        return null;
    }
}

[System.Serializable]
public class StageSceneItem : BaseItem
{
    public Screen scene;
    public Sprite numberImage;
    public Sprite sampleImage;
}