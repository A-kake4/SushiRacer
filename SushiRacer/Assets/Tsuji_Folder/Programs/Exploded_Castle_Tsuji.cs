using UnityEngine;
using UnityEngine.SceneManagement;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class Exploded_Castle_Tsuji : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 1.0f; // 振幅、揺れの幅
    [SerializeField]
    private float frequency = 1.0f; // 周波数、揺れの速さ
    Vector3 startPosition;

    [SerializeField]
    private float fallSpeed = 5.0f; // 落下速度
    private float timer = 0.0f;
    private float breakTime = 1.0f; // 揺れの時間

    private Vector3 swayDirection = new Vector3(-1.0f, 0.0f, 1.0f).normalized; // 揺れの方向

    [SerializeField]
    private GoalAction_Tsuji goalAction = null;

    //[SerializeField]
    //SceneAsset sceneAsset = null;   // シーンアセットをインスペクターで設定

    // [SerializeField, HideInInspector] private string sceneName; // 実行時に使うシーン名
    [SerializeField] private string sceneName; // 実行時に使うシーン名

    [SerializeField]
    private float finishHeight = -50.0f;    // 城が沈んで終了したい高さ
    private bool isFinish = false;          // 終了フラグ

    [SerializeField]
    private float finishWaitTime = 2.0f; // 終了後の待機時間
    private float finishTimer = 0.0f;

    private bool isExploded = false;
    [SerializeField]
    private float explodeStartTime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (goalAction == null) return;

        if (goalAction.GetGoalFlag() == true && isFinish == false)
        {
            timer += Time.fixedDeltaTime;
            if (timer > explodeStartTime)
            {
                breakTime += Time.fixedDeltaTime;
                float offset = Mathf.Sin(breakTime * frequency) * amplitude;
                float fallOffset = breakTime * fallSpeed;

                transform.position = startPosition + swayDirection * offset + new Vector3(0, -fallOffset, 0);
                isExploded = true;
            }
        }
        
        if(transform.position.y < finishHeight)
        {
            isFinish = true;
        }


        if(isFinish == true)
        {
            finishTimer += Time.fixedDeltaTime;
            if (finishWaitTime < finishTimer)
            {
                LoadScene();
            }
        }
    }

    //#if UNITY_EDITOR
    //    private void OnValidate()
    //    {
    //        //if (sceneAsset != null)
    //        //{
    //        //    sceneName = sceneAsset.name; // エディタ上でシーン名を自動取得
    //        //}
    //    }
    //#endif

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // 実行時は文字列でロード
        }
    }

    public bool GetExplodedFlag()
    {
        return isExploded;
    }

    public bool GetFinishFlag()
    {
        return isFinish;
    }
}