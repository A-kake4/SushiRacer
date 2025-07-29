using UnityEngine;

public class GameModeManager : SingletonMonoBehaviour<GameModeManager>
{
    [SerializeField, ReadOnly]
    private string nowGameMode;


    protected override void AwakeSingleton()
    {
    }

    protected override void OnDestroySingleton()
    {
    }
}
