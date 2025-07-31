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
            if ( value == null )
                nowGameMode = value;
        }
              
        get
        {
            return nowGameMode;
        }
    }

    // �v���C���[���Ƃ̓��̓f�[�^�Ǘ�
    private readonly Dictionary<int, Dictionary<string, ActionData>> playerActionDataDict = new();

    // �v���C���[���Ƃ�InputUser�Ǘ�
    private readonly Dictionary<int, InputUser> playerInputUsers = new();

    /// <summary>
    /// �v���C���[���Ƃ�InputUser���Z�b�g�A�b�v
    /// </summary>
    public void RegisterPlayerDevice( int playerIndex, InputDevice device )
    {
        InputUser user;
        if ( playerInputUsers.ContainsKey( playerIndex ) )
        {
            user = playerInputUsers[playerIndex];
            InputUser.PerformPairingWithDevice( device, user );
            // �������[�U�[�Ƀf�o�C�X�ǉ��̂�
            return;
        }

        user = InputUser.CreateUserWithoutPairedDevices();
        user.AssociateActionsWithUser( inputActionAsset );
        InputUser.PerformPairingWithDevice( device, user );
        playerInputUsers[playerIndex] = user;

        // �R�[���o�b�N�o�^�Ȃǂ͏���̂�
        foreach ( var actionMap in inputActionAsset.actionMaps )
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
        // �f�t�H���g��InputUser�͓o�^���Ȃ��iRegisterPlayerDevice�ŊǗ��j
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
    /// �v���C���[�ԍ��E�A�N�V�����}�b�v���E�A�N�V������������͒l���擾
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
    /// �v���C���[�ԍ��E�A�N�V�����}�b�v���E�A�N�V����������t�F�[�Y���擾
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
