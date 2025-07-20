using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneManager : SingletonMonoBehaviour<StageSceneManager>
{
    [SerializeField]
    private StageSceneScriptableObject stageSceneData;

    protected override void AwakeSingleton()
    {
    }

    public void LoadStageScene(int index)
    {
        // Load the stage scene
        SceneManager.LoadScene( stageSceneData.GetStageScene( index ).id );
    }

    protected override void OnDestroySingleton()
    {
    }
}
