using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//----------------------------------------
// 生成日：2025/04/16
// 更新日：2025/04/16
// 更新者：藤原
//----------------------------------------
// timeScaleを変更することで、時間の流れを制御するクラス
// 外部のクラスから呼び出すことで、そのクラス毎の時間の数値を記録
// 他のクラスとの時間の流れの処理が被っていても
// それぞれのクラスで時間の流れを制御できるようにする
//-----------------------------------------
// 使い方
// 引数としてMonoBehaviourを渡すことで、どのクラスからの呼び出しかを特定する
//
// 1. 呼び出すコンポーネントと速度を引数にして、時間の早さを変更する
// 例）TimeManager.Instance.SetTimeScale(this, 0.5f);
//
// 2. 変更に制限時間をかける場合
//    SetFixedTimeLimit や
//    SetUnfixedTimeLimit で制限時間を設定する
// 例） TimeManager.Instance.SetFixedTimeLimit(this, 5.0f);
// 又は TimeManager.Instance.SetUnfixedTimeLimit(this, 5.0f);
//
// 3. 手動でも元に戻せる
// 例）TimeManager.Instance.RemoveTimeScale(this);
//
// 4. シーンを変える時は初期化する
// 例）TimeManager.Instance.InitTimeScale();

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    /// <summary>
    /// 時間スケールを記録するための辞書
    /// </summary>
    private readonly Dictionary<int, float> timeScaleList = new();

    /// <summary>
    /// ゲーム内時間の経過で解除するための辞書
    /// </summary>
    private readonly Dictionary<int, float> fixedTimeLimitList = new();

    /// <summary>
    /// 実時間での経過で解除するための辞書
    /// </summary>
    private readonly Dictionary<int, float> unfixedTimeLimitList = new();

    // デバッグ中にinspector上で表記するための変数
    [SerializeField, ReadOnly]
    private float totalTimeScale = 1.0f;

    /// <summary>
    /// 初期化
    /// </summary>
    protected override void AwakeSingleton()
    {
        InitTimeScale();
    }

    /// <summary>
    /// オブジェクトの時間スケールを設定
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    /// <param name="timeScale">設定するスケール</param>
    public void SetTimeScale( MonoBehaviour mono, float timeScale )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetTimeScaleに渡されたMonoBehaviourがnullです。" );
            return;
        }

        int monoId = mono.GetInstanceID();
        timeScaleList[monoId] = Mathf.Clamp( timeScale, 0.0f, 10.0f ); // スケール値をクランプ
        UpdateTimeScale();
    }

    /// <summary>
    /// 固定時間制限を設定
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    /// <param name="timeLimit">設定する時間制限</param>
    public void SetFixedTimeLimit( MonoBehaviour mono, float timeLimit )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetFixedTimeLimitに渡されたMonoBehaviourがnullです。" );
            return;
        }

        int monoId = mono.GetInstanceID();
        fixedTimeLimitList[monoId] = Mathf.Max( timeLimit, 0.0f ); // 時間制限を正の値に
    }

    /// <summary>
    /// 非固定時間制限を設定
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    /// <param name="timeLimit">設定する時間制限</param>
    public void SetUnfixedTimeLimit( MonoBehaviour mono, float timeLimit )
    {
        if ( mono == null )
        {
            Debug.LogError( "SetUnfixedTimeLimitに渡されたMonoBehaviourがnullです。" );
            return;
        }

        int monoId = mono.GetInstanceID();
        unfixedTimeLimitList[monoId] = Mathf.Max( timeLimit, 0.0f ); // 時間制限を正の値に
    }

    /// <summary>
    /// 記録しておいた時間スケールを削除
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    public void RemoveTimeScale( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "RemoveTimeScaleに渡されたMonoBehaviourがnullです。" );
            return;
        }

        RemoveTimeScale( mono.GetInstanceID(), true );
    }

    public void RemoveTimeScale( int monoId, bool showWarning = false )
    {
        bool removed = timeScaleList.Remove(monoId);
        bool fixedRemoved = fixedTimeLimitList.Remove(monoId);
        bool unfixedRemoved = unfixedTimeLimitList.Remove(monoId);

        if ( showWarning && !removed && !fixedRemoved && !unfixedRemoved )
        {
            Debug.LogWarning( $"指定されたオブジェクト (ID: {monoId}) の時間スケールが見つかりません。" );
        }
        UpdateTimeScale();
    }

    /// <summary>
    /// オブジェクトが指定している時間スケールを取得
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    public float GetTimeScale( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "GetTimeScaleに渡されたMonoBehaviourがnullです。" );
            return 1.0f;
        }

        return timeScaleList.TryGetValue( mono.GetInstanceID(), out float timeScale ) ? timeScale : 1.0f;
    }

    /// <summary>
    /// オブジェクトが指定している時間制限を取得
    /// </summary>
    /// <param name="mono">呼び出す側のコンポーネント(this)</param>
    public float GetTimeLimit( MonoBehaviour mono )
    {
        if ( mono == null )
        {
            Debug.LogError( "GetTimeLimitに渡されたMonoBehaviourがnullです。" );
            return 1.0f;
        }

        bool hasFixed = fixedTimeLimitList.TryGetValue(mono.GetInstanceID(), out float fixedLimit);
        bool hasUnfixed = unfixedTimeLimitList.TryGetValue(mono.GetInstanceID(), out float unfixedLimit);

        if ( hasFixed && hasUnfixed )
        {
            return Mathf.Min( fixedLimit, unfixedLimit );
        }
        else if ( hasFixed )
        {
            return fixedLimit;
        }
        else if ( hasUnfixed )
        {
            return unfixedLimit;
        }
        else
        {
            return 1.0f;
        }
    }

    /// <summary>
    /// 全体の時間スケールを計算
    /// </summary>
    public float TotalTimeScale
    {
        get
        {
            // 全ての時間スケールを掛け合わせて総合的な時間スケールを計算
            float timeScale = 1.0f;
            foreach ( var scale in timeScaleList.Values )
            {
                timeScale *= scale;
            }

            return timeScale;
        }
    }


    /// <summary>
    /// 時間スケールを更新
    /// </summary>
    private void UpdateTimeScale()
    {
        Time.timeScale = Mathf.Clamp( TotalTimeScale, 0.0f, 10.0f ); // スケール値をクランプ
#if UNITY_EDITOR
        totalTimeScale = Time.timeScale;
#endif
    }

    /// <summary>
    /// LateUpdateで時間制限を処理
    /// </summary>
    private void LateUpdate()
    {
        ProcessTimeLimits( fixedTimeLimitList, Time.deltaTime );
        ProcessTimeLimits( unfixedTimeLimitList, Time.unscaledDeltaTime );
    }

    /// <summary>
    /// 時間制限を処理
    /// </summary>
    private void ProcessTimeLimits( Dictionary<int, float> timeLimitList, float deltaTime )
    {
        if (timeLimitList.Count == 0) return;
        var keys = timeLimitList.Keys.ToArray();
        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i];
            if (timeLimitList.TryGetValue(key, out float remainingTime))
            {
                remainingTime -= deltaTime;
                if (remainingTime <= 0f)
                {
                    RemoveTimeScale(key, false); // 警告を出さない
                }
                else
                {
                    timeLimitList[key] = remainingTime;
                }
            }
        }
    }

    protected override void OnDestroySingleton()
    {
        // 必要に応じてリソース解放
    }

    /// <summary>
    /// 初期化時に時間スケールをリセット
    /// </summary>
    public void InitTimeScale()
    {
        timeScaleList.Clear();
        fixedTimeLimitList.Clear();
        unfixedTimeLimitList.Clear();
        UpdateTimeScale();
    }
}
