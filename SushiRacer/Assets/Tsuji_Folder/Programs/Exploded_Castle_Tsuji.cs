using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Exploded_Castle_Tsuji : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 1.0f; // 振幅、揺れの幅
    [SerializeField]
    private float frequency = 1.0f; // 周波数、揺れの速さ
    Vector3 startPosition;

    [SerializeField]
    private float fallSpeed = 5.0f; // 落下速度

    [SerializeField]
    private GoalAction_Tsuji goalAction = null;

    [SerializeField]
    SceneAsset sceneAsset = null;   // シーンアセットをインスペクターで設定

    [SerializeField, HideInInspector] private string sceneName; // 実行時に使うシーン名

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (goalAction == null) return;

        //if (goalAction.GetGoalFlag() == true)
        //{
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;
            float fallOffset = Time.time * fallSpeed;

            transform.position = startPosition + new Vector3(0, -fallOffset, offset);
       // }

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //if (sceneAsset != null)
        //{
        //    sceneName = sceneAsset.name; // エディタ上でシーン名を自動取得
        //}
    }
#endif

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); // 実行時は文字列でロード
        }
    }

}