using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Metadata;

public class PlayerSelectManager : SingletonMonoBehaviour<PlayerSelectManager>
{
    [SerializeField, Header( "�v���C���[�I���̍ő�l��" )]
    private int maxPlayerCount = 4; // �v���C���[�I���̍ő�l��
    [SerializeField, Header( "�v���C���[�I���f�[�^" )]
    private List<PlayerSelectData> playerSelectDataList = new();

    protected override void AwakeSingleton()
    {
        // �f�o�C�X�̕ύX���Ď�
        InputSystem.onDeviceChange += OnDeviceChange;

        // ���݂̃f�o�C�X�ꗗ�����O�o��
        var devices = InputSystem.devices;
        var sb = new StringBuilder();
        sb.AppendLine( "���ݐڑ�����Ă���f�o�C�X�ꗗ" );

        for (var i = 0; i < devices.Count; i++)
        {
            sb.AppendLine( $" - {devices[i]}" );
        }
        print( sb );

        // �v���C���[�f�[�^�̏����ݒ�
        InitializedAllDevices();
    }

    protected override void OnDestroySingleton()
    {
        // �f�o�C�X�ύX�̊Ď�����
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void InitializedAllDevices()
    {
        // �v���C���[�I���f�[�^���������i�����f�[�^���N���A���AmaxPlayerCount���p�ӂ���j
        playerSelectDataList.Clear();
        for (int i = 0; i < maxPlayerCount; i++)
        {
            // InputDevice �� new �ł��Ȃ����߁A�����l�� null �ɂ��܂�
            playerSelectDataList.Add( new PlayerSelectData() { device = null, characterIndex = 0 } );
        }

        // �ڑ�����Ă���f�o�C�X�ꗗ����AGamepad��Keyboard�̂ݎ擾
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

        // �e�v���C���[�Ƀf�o�C�X�����蓖��
        for (int i = 0; i < maxPlayerCount; i++)
        {
            if (availableGamepads.Count > 0)
            {
                // Gamepad������ΗD�悵�Ċ��蓖��
                var gamepad = availableGamepads[0];
                availableGamepads.RemoveAt( 0 );
                playerSelectDataList[i].device = gamepad;
                Debug.Log( $"�v���C���[{i}��Gamepad {gamepad}�����蓖�Ă܂����B" );
            }
            else if (keyboard != null)
            {
                // Gamepad���Ȃ���΃L�[�{�[�h�����蓖��
                playerSelectDataList[i].device = keyboard;
                Debug.Log( $"�v���C���[{i}�ɃL�[�{�[�h {keyboard}�����蓖�Ă܂����B" );
            }
            else
            {
                playerSelectDataList[i].device = null;
                Debug.LogWarning( $"�v���C���[{i}�Ɋ��蓖�Ă�f�o�C�X������܂���B" );
            }
            // �f�o�C�X����ݒ�
            playerSelectDataList[i].deviceName = playerSelectDataList[i].device != null ? playerSelectDataList[i].device.displayName : "���ݒ�";
        }
    }

    private void UpdateAllDevices()
    {
        // �ڑ�����Ă���f�o�C�X�ꗗ���X�V
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
        // �v���C���[�I���f�[�^���ď�����
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

    // �f�o�C�X�̕ύX�����m�������̏���
    private void OnDeviceChange( InputDevice device, InputDeviceChange change )
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log( $"�f�o�C�X {device} ���ڑ�����܂����B" );
                break;

            case InputDeviceChange.Disconnected:
                Debug.Log( $"�f�o�C�X {device} ���ؒf����܂����B" );
                break;

            case InputDeviceChange.Reconnected:
                Debug.Log( $"�f�o�C�X {device} ���Đڑ�����܂����B" );
                break;

            default:
                // �ڑ���ؒf�ȊO�̕ύX�͖���
                return;
        }

        // �ڑ���ؒf���������ꍇ�́A�S�Ẵf�o�C�X���Ăу��O�o��
        UpdateAllDevices();
    }
}

[System.Serializable]
public class PlayerSelectData
{
    // �L�[�{�[�h��g�p���Ă�����̓f�o�C�X�̏��
    public InputDevice device = null;
    [Header( "�g�p���Ă���f�o�C�X" ), ReadOnly]
    public string deviceName = "���ݒ�";
    [Header( "�I��ł���L�����N�^�[" )]
    public int characterIndex = 0;
}