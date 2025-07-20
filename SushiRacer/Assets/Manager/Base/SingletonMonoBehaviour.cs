using UnityEngine;

//----------------------------------------  
// 作成日：2025/06/18 作成者：藤原  
// 更新日：2025/07/07 更新者：藤原  
//----------------------------------------  
// MonoBehaviourを継承したシングルトンの基底クラス  
// dontDestroyOnLoadの選択によってシーンをまたいでもインスタンスが破棄されないようにする  
// シーン内にインスタンスがない場合、ManagerSceneを追加してインスタンスを探す  
// それでもインスタンスがない場合、エラーログを出力する  
//----------------------------------------
public abstract class SingletonMonoBehaviour<T> : SingletonSealableMonoBehaviour
                                            where T : SingletonMonoBehaviour<T>
{
    [SerializeField]
    protected bool dontDestroyOnLoad = true;

    private static T instance;

    /// <summary>
    /// シングルトンインスタンス取得
    /// </summary>
    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            if ( instance == null )
            {
                Debug.LogWarning( $"SingletonMonoBehaviour<{typeof( T ).Name}>のインスタンスが存在しません。" );
            }
#endif
            return instance;
        }
    }
    protected sealed override void Awake()
    {
        if ( instance == null )
        {
            instance = this as T;
            if ( dontDestroyOnLoad )
            {
                DontDestroyOnLoad( gameObject );
            }
            AwakeSingleton();
        }
        else if ( instance != this )
        {
            Destroy( gameObject );
            return;
        }
    }

    /// <summary>
    /// シングルトンの初期化処理。継承先でオーバーライドしてください。
    /// </summary>
    protected abstract void AwakeSingleton();

    /// <summary>
    /// シングルトンの終了処理。継承先でオーバーライドしてください。
    /// </summary>
    protected abstract void OnDestroySingleton();

    protected sealed override void OnDestroy()
    {
        if ( instance == this )
        {
            OnDestroySingleton();
            instance = null; // インスタンス解放
        }
    }
}
