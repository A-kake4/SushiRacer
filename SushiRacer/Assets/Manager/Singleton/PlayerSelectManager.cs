using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Metadata;

public class PlayerSelectManager : SingletonMonoBehaviour<PlayerSelectManager>
{
    [SerializeField, Header( "プレイヤー選択の最大人数" )]
    private int maxPlayerCount = 4; // プレイヤー選択の最大人数
    [SerializeField, Header( "プレイヤー選択データ" )]
    private List<PlayerSelectData> playerSelectDataList = new();

    protected override void AwakeSingleton()
    {
        // デバイスの変更を監視
        InputSystem.onDeviceChange += OnDeviceChange;

        // 現在のデバイス一覧をログ出力
        var devices = InputSystem.devices;
        var sb = new StringBuilder();
        sb.AppendLine( "現在接続されているデバイス一覧" );

        for (var i = 0; i < devices.Count; i++)
        {
            sb.AppendLine( $" - {devices[i]}" );
        }
        print( sb );

        // プレイヤーデータの初期設定
        InitializedAllDevices();
    }

    protected override void OnDestroySingleton()
    {
        // デバイス変更の監視解除
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void InitializedAllDevices()
    {
        // プレイヤー選択データを初期化（既存データをクリアし、maxPlayerCount分用意する）
        playerSelectDataList.Clear();
        for (int i = 0; i < maxPlayerCount; i++)
        {
            // InputDevice は new できないため、初期値は null にします
            playerSelectDataList.Add( new PlayerSelectData() { device = null, characterIndex = 0 } );
        }

        // 接続されているデバイス一覧から、GamepadとKeyboardのみ取得
        var allDevices = InputSystem.devices;
        var availableGamepads = new List<Gamepad>();
        Keyboard keyboard = Keyboard.current;

        foreach (var device in allDevices)
        {
            if (device is Gamepad gamepad)
            {
                availableGamepads.Add( gamepad );
            }
        }

        // 各プレイヤーにデバイスを割り当て
        for (int i = 0; i < maxPlayerCount; i++)
        {
            if (availableGamepads.Count > 0)
            {
                // Gamepadがあれば優先して割り当て
                var gamepad = availableGamepads[0];
                availableGamepads.RemoveAt( 0 );
                playerSelectDataList[i].device = gamepad;
                Debug.Log( $"プレイヤー{i}にGamepad {gamepad}を割り当てました。" );
            }
            else if (keyboard != null)
            {
                // Gamepadがなければキーボードを割り当て
                playerSelectDataList[i].device = keyboard;
                Debug.Log( $"プレイヤー{i}にキーボード {keyboard}を割り当てました。" );
            }
            else
            {
                playerSelectDataList[i].device = null;
                Debug.LogWarning( $"プレイヤー{i}に割り当てるデバイスがありません。" );
            }
            // デバイス名を設定
            playerSelectDataList[i].deviceName = playerSelectDataList[i].device != null ? playerSelectDataList[i].device.displayName : "未設定";
        }
    }

    private void UpdateAllDevices()
    {
        // 接続されているデバイス一覧を更新
        var allDevices = InputSystem.devices;
        var availableGamepads = new List<Gamepad>();
        Keyboard keyboard = Keyboard.current;
        foreach (var device in allDevices)
        {
            if (device is Gamepad gamepad)
            {
                availableGamepads.Add( gamepad );
            }
        }
        // プレイヤー選択データを再初期化
        InitializedAllDevices();
    }

    public InputDevice GetPlayerDevice( int playerIndex )
    {
        if ( playerIndex < 0 || playerIndex >= playerSelectDataList.Count )
        {
            Debug.LogError( $"Invalid player index: {playerIndex}" );
            return null;
        }
        return playerSelectDataList[playerIndex].device;
    }

    // デバイスの変更を検知した時の処理
    private void OnDeviceChange( InputDevice device, InputDeviceChange change )
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log( $"デバイス {device} が接続されました。" );
                break;

            case InputDeviceChange.Disconnected:
                Debug.Log( $"デバイス {device} が切断されました。" );
                break;

            case InputDeviceChange.Reconnected:
                Debug.Log( $"デバイス {device} が再接続されました。" );
                break;

            default:
                // 接続や切断以外の変更は無視
                return;
        }

        // 接続や切断があった場合は、全てのデバイスを再びログ出力
        UpdateAllDevices();
    }
}

[System.Serializable]
public class PlayerSelectData
{
    // キーボードや使用している入力デバイスの情報
    public InputDevice device = null;
    [Header( "使用しているデバイス" ), ReadOnly]
    public string deviceName = "未設定";
    [Header( "選んでいるキャラクター" )]
    public int characterIndex = 0;
}