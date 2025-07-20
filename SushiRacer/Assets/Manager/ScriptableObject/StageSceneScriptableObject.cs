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
        // LINQを使用して、指定されたステージIDに一致するアイテムを検索
        var stageScene = items[stageIndex];
        if ( stageScene != null )
        {
            return stageScene;
        }

        // 指定されたIDのステージが見つからない場合はnullを返す
        Debug.LogError( $"[StageSceneScriptableObject] Stage ID {stageIndex} not found." );
        return null;
    }

    public StageSceneItem GetStageScene( string stageId )
    {
        // LINQを使用して、指定されたステージIDに一致するアイテムを検索
        var stageScene = System.Array.Find( items, item => item.id == stageId );
        if ( stageScene != null )
        {
            return stageScene;
        }
        // 指定されたIDのステージが見つからない場合はnullを返す
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