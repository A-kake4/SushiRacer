using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField]
    private InputActionAsset inputActionAsset;

    public InputActionAsset InputActionAsset => inputActionAsset;
    [SerializeField, ReadOnly]
    private string nowGameMode;

    public string CurrentGameMode
    {
        set
        {
            if (value != null)
            {
                nowGameMode = value;
            }
        }
              
        get
        {
            return nowGameMode;
        }
    }

    // プレイヤーごとの入力データ管理
    private readonly Dictionary<int, Dictionary<string, ActionData>> playerActionDataDict = new();

    // プレイヤーごとのInputUser管理
    private readonly Dictionary<int, InputUser> playerInputUsers = new();

    /// <summary>
    /// プレイヤーごとのInputUserをセットアップ
    /// 例： RegisterPlayerDevice(1, Gamepad.current);
    /// キーボード入力の場合は、Gamepad.currentをnullにするか、Keyboard.currentを使用
    /// </summary>
    public void RegisterPlayerDevice( int playerIndex, InputDevice device )
    {
        InputUser user;
        if ( playerInputUsers.ContainsKey( playerIndex ) )
        {
            user = playerInputUsers[playerIndex];
            InputUser.PerformPairingWithDevice( device, user );
            // キーボードが登録された場合、Mouse.currentが存在すればマウスも追加でペアリング
            if ( device is Keyboard && Mouse.current != null )
            {
                InputUser.PerformPairingWithDevice( Mouse.current, user );
            }
            return;
        }

        // プレイヤー専用のInputActionAssetを生成
        var playerActionAsset = Object.Instantiate(inputActionAsset);

        user = InputUser.CreateUserWithoutPairedDevices();
        user.AssociateActionsWithUser( playerActionAsset );
        InputUser.PerformPairingWithDevice( device, user );
        // キーボードの場合は、マウスも自動でペアリング
        if ( device is Keyboard && Mouse.current != null )
        {
            InputUser.PerformPairingWithDevice( Mouse.current, user );
        }
        playerInputUsers[playerIndex] = user;

        // コールバック登録は初回のみ
        foreach ( var actionMap in playerActionAsset.actionMaps )
        {
            foreach ( var action in actionMap.actions )
            {
                action.performed += ctx => OnActionPerformed( ctx, playerIndex );
                action.started += ctx => OnActionStarted( ctx, playerIndex );
                action.canceled += ctx => OnActionCanceled( ctx, playerIndex );
                action.Enable();
            }
        }

        if ( !playerActionDataDict.ContainsKey( playerIndex ) )
            playerActionDataDict[playerIndex] = new Dictionary<string, ActionData>();
    }

    protected override void AwakeSingleton()
    {
        // デフォルトのInputUserは登録しない（RegisterPlayerDeviceで管理）
    }

    protected override void OnDestroySingleton()
    {
        foreach ( var user in playerInputUsers.Values )
        {
            user.UnpairDevicesAndRemoveUser();
        }
    }

    private void OnActionStarted( InputAction.CallbackContext context, int playerIndex )
    {
        SaveActionData( context, playerIndex );
    }

    private void OnActionPerformed( InputAction.CallbackContext context, int playerIndex )
    {
        SaveActionData( context, playerIndex );
    }

    private void OnActionCanceled( InputAction.CallbackContext context, int playerIndex )
    {
        SaveActionData( context, playerIndex );
    }

    private void SaveActionData( InputAction.CallbackContext context, int playerIndex )
    {
        var action = context.action;
        object value = null;

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
                value = context.ReadValueAsObject();
                break;
        }

        string key = $"{action.actionMap.name}/{action.name}";

        var actionData = new ActionData
        {
            Value = value,
            Phase = action.phase
        };

        playerActionDataDict[playerIndex][key] = actionData;
    }

    /// <summary>
    /// プレイヤー番号・アクションマップ名・アクション名から入力値を取得
    /// </summary>
    public T GetActionValue<T>( int playerIndex, string actionMapName, string actionName )
    {
        if ( playerActionDataDict.TryGetValue( playerIndex, out var dict ) )
        {
            string key = $"{actionMapName}/{actionName}";
            if ( dict.TryGetValue( key, out var actionData ) && actionData.Value is T tValue )
            {
                return tValue;
            }
        }
        return default;
    }

    /// <summary>
    /// プレイヤー番号・アクションマップ名・アクション名からフェーズを取得
    /// 呼び出し例： GetActionPhase(1, "MainGame", "Jump")
    /// if( GetActionPhase(1, "MainGame", "Jump") == InputActionPhase.Performed ) { ... }
    /// </summary>
    public InputActionPhase GetActionPhase( int playerIndex, string actionMapName, string actionName )
    {
        if ( playerActionDataDict.TryGetValue( playerIndex, out var dict ) )
        {
            string key = $"{actionMapName}/{actionName}";
            if ( dict.TryGetValue( key, out var actionData ) )
            {
                return actionData.Phase;
            }
        }
        return InputActionPhase.Disabled;

    }

    private class ActionData
    {
        public object Value { get; set; }
        public InputActionPhase Phase { get; set; }
    }
}
