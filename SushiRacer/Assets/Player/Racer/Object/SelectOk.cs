using System.Collections.Generic;
using UnityEngine;

public class SelectOk : MonoBehaviour
{
    public static SelectOk Instance { get; private set; }

    private List<PlayerSelectView> playerSelectViews = new List<PlayerSelectView>();
    private int readyCount = 0;

    [SerializeField]
    private int maxDelayCount = 0;

    private int delayCount = 0;

    [SerializeField]
    private SceneJump[] sceneJump;

    private void Awake()
    {
         if (Instance == null)
         {
              Instance = this;
         }
    }

    public void SetSelectView( PlayerSelectView playerSelectView )
    {
        if (!playerSelectViews.Contains( playerSelectView ))
        {
            playerSelectViews.Add( playerSelectView );
        }
    }


    private void FixedUpdate()
    {
        // �S���������������Ă��邩�m�F
        readyCount = 0;
        foreach (var view in playerSelectViews)
        {
            if (view.IsReady)
            {
                readyCount++;
            }
        }
        if (readyCount == playerSelectViews.Count && readyCount > 0)
        {
            delayCount++;
            if (delayCount >= maxDelayCount)
            {
                // �����_���ŃV�[���J��
                int index = Random.Range( 0, sceneJump.Length );
                sceneJump[index].JumpToTargetScene();
            }
        }
        else
        {
            delayCount = 0;
        }
    }
}
