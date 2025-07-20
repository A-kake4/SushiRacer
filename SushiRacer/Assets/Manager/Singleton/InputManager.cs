using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//----------------------------------------
// 生成日：2025/04/16
// 更新日：2025/07/01
// 更新者：藤原
// 更新内容：このプログラムの解説を追加
//----------------------------------------
/*
 * InputManager クラスの概要
 * -------------------------
 * 
 * InputManager はシングルトンパターンを採用した入力管理クラスです。
 * InputActionAsset を使用して、全ての入力アクションに対するコールバックを管理します。
 * このクラスにより、複数のスクリプトから統一的に入力状態を取得・管理することが可能です。
 * 
 * 主な機能
 * --------
 * ・入力アクションの開始、実行、キャンセル時にデータを保存
 * ・指定したアクションの現在の値やフェーズを取得
 * 
 * 使用可能な関数
 * --------------
 * 1. GetActionValue<T>(string actionMapName, string actionName)
 *    ・説明: 指定したアクションマップ名とアクション名から入力値を取得します。
 *    ・型パラメータ: T は取得したい値の型です（例: bool, Vector2）。
 *    ・使用例:
 *          bool isJumpPressed = InputManager.Instance.GetActionValue<bool>("Player", "Jump");
 *          Vector2 movement = InputManager.Instance.GetActionValue<Vector2>("Player", "Move");
 * 
 * 2. GetActionPhase(string actionMapName, string actionName)
 *    ・説明: 指定したアクションマップ名とアクション名からアクションの
 *            現在のフェーズ（状態）を取得します。これにより、アクションが開始された、
 *            実行された、またはキャンセルされたかを確認し、
 *            入力に応じた処理を実装する際に役立ちます。
 *    ・使用例:
 *          InputActionPhase jumpPhase = InputManager.Instance.GetActionPhase("Player", "Jump");
 *          if (jumpPhase == InputActionPhase.Started)
 *          {
 *              // ジャンプ処理を実行
 *          }
 * 
 * フェーズについて
 * --------------
 * InputActionPhase は入力アクションの現在の状態を示す列挙型です。主なフェーズは以下の通りです。
 * ・Started: アクションが開始された状態
 * ・Performed: アクションが完了した状態
 * ・Canceled: アクションがキャンセルされた状態
 * 
 * 各フェーズを利用して、入力の詳細な制御や反応を実装することができます。
 * 
 * 取得できる値の種類
 * ----------------
 * GetActionValue<T> メソッドを使用して取得できる値の型はアクションの設定に依存します。一般的な型は以下の通りです。
 * ・bool: ボタンの押下状態（例: Jump アクション）
 * ・Vector2: 2D ベクトル入力（例: Move アクション）
 * ・Vector3: 3D ベクトル入力（必要に応じて）
 * ・float: アナログ入力や軸入力（例: スティックの傾き）
 * 
 * 必要に応じて、他の型もサポート可能です。アクションの期待するコントロールタイプに応じて適切な型を選択してください。
 * 
 * 使用上の注意
 * ------------
 * ・このクラスはシングルトンとして設計されているため、シーン内に複数のインスタンスが存在しないようにしてください。
 * ・InputActionAsset はインスペクターから設定する必要があります。
 * 
 * 記述例
 * -------
 * 以下は、InputManager を使用してプレイヤーの移動とジャンプを処理する例です。
 * 
 * public class PlayerController : MonoBehaviour
 * {
 *     void Update()
 *     {
 *         Vector2 moveInput = InputManager.Instance.GetActionValue<Vector2>("Player", "Move");
 *         bool jump = InputManager.Instance.GetActionValue<bool>("Player", "Jump");
 *         
 *         // 移動処理
 *         Move(moveInput);
 *         
 *         // ジャンプ処理
 *         if (jump)
 *         {
 *             Jump();
 *         }
 *     }
 *     
 *     void Move(Vector2 direction)
 *     {
 *         // 移動ロジック
 *     }
 *     
 *     void Jump()
 *     {
 *         // ジャンプロジック
 *     }
 * }
 */
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    // インスペクターから設定可能な InputActionAsset
    [SerializeField]
    private InputActionAsset inputActionAsset;

    /// <summary>
    /// InputActionAsset を公開するプロパティ
    /// </summary>
    public InputActionAsset InputActionAsset => inputActionAsset;

    // Inspectorで選択可能なゲームモード（アクションマップ名）
    [SerializeField, ReadOnly]
    private string nowGameMode;

    // アクションマップ名とアクション名をキーとして、入力値と状態を保存する辞書
    private readonly Dictionary<string, ActionData> actionDataDict = new();

    /// <summary>
    /// 現在のゲームモードを取得・設定します。
    /// アクションマップ名を示す文字列です。
    /// </summary>
    public string CurrentGameMode
    {
        get => nowGameMode;
        set 
        {
            if ( inputActionAsset == null )
            {
                Debug.LogError( "InputActionAssetが設定されていません。" );
                return;
            }

            // 指定されたアクションマップ名を検索
            var actionMap = inputActionAsset.FindActionMap(value, false);
            if ( actionMap != null )
            {
                nowGameMode = value;
            }
            else
            {
                Debug.LogError( $"アクションマップ名 '{value}' が InputActionAsset に存在しません。" );
            }
        }
    }

    /// <summary>
    /// Awake メソッドはオブジェクトの初期化時に一度呼ばれます。
    /// シングルトンの設定と全アクションへのコールバックの登録を行います。
    /// </summary>
    protected override void AwakeSingleton()
    {
        // 全てのアクションマップとアクションに対してコールバックを登録し、有効化します
        foreach ( var actionMap in inputActionAsset.actionMaps )
        {
            foreach ( var action in actionMap.actions )
            {
                action.started += OnActionStarted;
                action.performed += OnActionPerformed;
                action.canceled += OnActionCanceled;
                action.Enable();
            }
        }
    }

    /// <summary>
    /// オブジェクトが破棄される際に呼ばれます。
    /// 登録したコールバックの解除とアクションの無効化を行います。
    /// </summary>
    protected override void OnDestroySingleton()
    {
        // 全てのアクションマップとアクションからコールバックを解除し、無効化します
        foreach ( var actionMap in inputActionAsset.actionMaps )
        {
            foreach ( var action in actionMap.actions )
            {
                action.started -= OnActionStarted;
                action.performed -= OnActionPerformed;
                action.canceled -= OnActionCanceled;
                action.Disable();
            }
        }
    }

    /// <summary>
    /// アクションが開始されたときに呼び出されるコールバック。
    /// 入力データを保存します。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト情報</param>
    private void OnActionStarted( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// アクションが実行されたときに呼び出されるコールバック。
    /// 入力データを保存します。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト情報</param>
    private void OnActionPerformed( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// アクションがキャンセルされたときに呼び出されるコールバック。
    /// 入力データを保存します。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト情報</param>
    private void OnActionCanceled( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// 入力アクションのデータとそのフェーズを保存します。
    /// </summary>
    /// <param name="context">入力アクションのコンテキスト情報</param>
    private void SaveActionData( InputAction.CallbackContext context )
    {
        var action = context.action;
        object value = null;

        // アクションの期待するコントロールタイプに応じて値を取得
        switch ( action.expectedControlType )
        {
            case "Button":
                value = context.ReadValue<float>() > 0.5f;
                break;
            case "Vector2":
                value = context.ReadValue<Vector2>();
                break;
            case "Vector3":
                value = context.ReadValue<Vector3>();
                break;
            case "Axis":
            case "Analog":
                value = context.ReadValue<float>();
                break;
            default:
                // 他のコントロールタイプが必要な場合はここに追加
                value = context.ReadValueAsObject();
                break;
        }

        // アクションマップ名とアクション名を組み合わせてユニークなキーを作成
        string key = $"{action.actionMap.name}/{action.name}";

        // アクションデータを更新または追加
        var actionData = new ActionData
        {
            Value = value,
            Phase = action.phase
        };

        actionDataDict[key] = actionData;
    }

    /// <summary>
    /// 指定したアクションマップ名とアクション名から入力値を取得します。
    /// </summary>
    /// <typeparam name="T">取得したい値の型</typeparam>
    /// <param name="actionMapName">アクションマップの名前</param>
    /// <param name="actionName">アクションの名前</param>
    /// <returns>指定した型の入力値。存在しない場合はデフォルト値。</returns>
    public T GetActionValue<T>( string actionMapName, string actionName )
    {
        string key = $"{actionMapName}/{actionName}";
        if ( actionDataDict.TryGetValue( key, out var actionData ) && actionData.Value is T tValue )
        {
            return tValue;
        }
        return default;
    }

    /// <summary>
    /// 指定したアクションマップ名とアクション名からアクションのフェーズを取得します。
    /// </summary>
    /// <param name="actionMapName">アクションマップの名前</param>
    /// <param name="actionName">アクションの名前</param>
    /// <returns>アクションの現在のフェーズ。存在しない場合は Disabled。</returns>
    public InputActionPhase GetActionPhase( string actionMapName, string actionName )
    {
        string key = $"{actionMapName}/{actionName}";
        if ( actionDataDict.TryGetValue( key, out var actionData ) )
        {
            return actionData.Phase;
        }
        return InputActionPhase.Disabled;
    }

    /// <summary>
    /// アクションのデータを保持する内部クラス。
    /// </summary>
    private class ActionData
    {
        /// <summary>
        /// 入力値を保持します。型はアクションの期待するコントロールタイプに依存します。
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// アクションの現在のフェーズを保持します。
        /// </summary>
        public InputActionPhase Phase { get; set; }
    }
}

