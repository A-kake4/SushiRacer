using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//----------------------------------------
// �������F2025/04/16
// �X�V���F2025/07/01
// �X�V�ҁF����
// �X�V���e�F���̃v���O�����̉����ǉ�
//----------------------------------------
/*
 * InputManager �N���X�̊T�v
 * -------------------------
 * 
 * InputManager �̓V���O���g���p�^�[�����̗p�������͊Ǘ��N���X�ł��B
 * InputActionAsset ���g�p���āA�S�Ă̓��̓A�N�V�����ɑ΂���R�[���o�b�N���Ǘ����܂��B
 * ���̃N���X�ɂ��A�����̃X�N���v�g���瓝��I�ɓ��͏�Ԃ��擾�E�Ǘ����邱�Ƃ��\�ł��B
 * 
 * ��ȋ@�\
 * --------
 * �E���̓A�N�V�����̊J�n�A���s�A�L�����Z�����Ƀf�[�^��ۑ�
 * �E�w�肵���A�N�V�����̌��݂̒l��t�F�[�Y���擾
 * 
 * �g�p�\�Ȋ֐�
 * --------------
 * 1. GetActionValue<T>(string actionMapName, string actionName)
 *    �E����: �w�肵���A�N�V�����}�b�v���ƃA�N�V������������͒l���擾���܂��B
 *    �E�^�p�����[�^: T �͎擾�������l�̌^�ł��i��: bool, Vector2�j�B
 *    �E�g�p��:
 *          bool isJumpPressed = InputManager.Instance.GetActionValue<bool>("Player", "Jump");
 *          Vector2 movement = InputManager.Instance.GetActionValue<Vector2>("Player", "Move");
 * 
 * 2. GetActionPhase(string actionMapName, string actionName)
 *    �E����: �w�肵���A�N�V�����}�b�v���ƃA�N�V����������A�N�V������
 *            ���݂̃t�F�[�Y�i��ԁj���擾���܂��B����ɂ��A�A�N�V�������J�n���ꂽ�A
 *            ���s���ꂽ�A�܂��̓L�����Z�����ꂽ�����m�F���A
 *            ���͂ɉ�������������������ۂɖ𗧂��܂��B
 *    �E�g�p��:
 *          InputActionPhase jumpPhase = InputManager.Instance.GetActionPhase("Player", "Jump");
 *          if (jumpPhase == InputActionPhase.Started)
 *          {
 *              // �W�����v���������s
 *          }
 * 
 * �t�F�[�Y�ɂ���
 * --------------
 * InputActionPhase �͓��̓A�N�V�����̌��݂̏�Ԃ������񋓌^�ł��B��ȃt�F�[�Y�͈ȉ��̒ʂ�ł��B
 * �EStarted: �A�N�V�������J�n���ꂽ���
 * �EPerformed: �A�N�V�����������������
 * �ECanceled: �A�N�V�������L�����Z�����ꂽ���
 * 
 * �e�t�F�[�Y�𗘗p���āA���͂̏ڍׂȐ���┽�����������邱�Ƃ��ł��܂��B
 * 
 * �擾�ł���l�̎��
 * ----------------
 * GetActionValue<T> ���\�b�h���g�p���Ď擾�ł���l�̌^�̓A�N�V�����̐ݒ�Ɉˑ����܂��B��ʓI�Ȍ^�͈ȉ��̒ʂ�ł��B
 * �Ebool: �{�^���̉�����ԁi��: Jump �A�N�V�����j
 * �EVector2: 2D �x�N�g�����́i��: Move �A�N�V�����j
 * �EVector3: 3D �x�N�g�����́i�K�v�ɉ����āj
 * �Efloat: �A�i���O���͂⎲���́i��: �X�e�B�b�N�̌X���j
 * 
 * �K�v�ɉ����āA���̌^���T�|�[�g�\�ł��B�A�N�V�����̊��҂���R���g���[���^�C�v�ɉ����ēK�؂Ȍ^��I�����Ă��������B
 * 
 * �g�p��̒���
 * ------------
 * �E���̃N���X�̓V���O���g���Ƃ��Đ݌v����Ă��邽�߁A�V�[�����ɕ����̃C���X�^���X�����݂��Ȃ��悤�ɂ��Ă��������B
 * �EInputActionAsset �̓C���X�y�N�^�[����ݒ肷��K�v������܂��B
 * 
 * �L�q��
 * -------
 * �ȉ��́AInputManager ���g�p���ăv���C���[�̈ړ��ƃW�����v�����������ł��B
 * 
 * public class PlayerController : MonoBehaviour
 * {
 *     void Update()
 *     {
 *         Vector2 moveInput = InputManager.Instance.GetActionValue<Vector2>("Player", "Move");
 *         bool jump = InputManager.Instance.GetActionValue<bool>("Player", "Jump");
 *         
 *         // �ړ�����
 *         Move(moveInput);
 *         
 *         // �W�����v����
 *         if (jump)
 *         {
 *             Jump();
 *         }
 *     }
 *     
 *     void Move(Vector2 direction)
 *     {
 *         // �ړ����W�b�N
 *     }
 *     
 *     void Jump()
 *     {
 *         // �W�����v���W�b�N
 *     }
 * }
 */
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    // �C���X�y�N�^�[����ݒ�\�� InputActionAsset
    [SerializeField]
    private InputActionAsset inputActionAsset;

    /// <summary>
    /// InputActionAsset �����J����v���p�e�B
    /// </summary>
    public InputActionAsset InputActionAsset => inputActionAsset;

    // Inspector�őI���\�ȃQ�[�����[�h�i�A�N�V�����}�b�v���j
    [SerializeField, ReadOnly]
    private string nowGameMode;

    // �A�N�V�����}�b�v���ƃA�N�V���������L�[�Ƃ��āA���͒l�Ə�Ԃ�ۑ����鎫��
    private readonly Dictionary<string, ActionData> actionDataDict = new();

    /// <summary>
    /// ���݂̃Q�[�����[�h���擾�E�ݒ肵�܂��B
    /// �A�N�V�����}�b�v��������������ł��B
    /// </summary>
    public string CurrentGameMode
    {
        get => nowGameMode;
        set 
        {
            if ( inputActionAsset == null )
            {
                Debug.LogError( "InputActionAsset���ݒ肳��Ă��܂���B" );
                return;
            }

            // �w�肳�ꂽ�A�N�V�����}�b�v��������
            var actionMap = inputActionAsset.FindActionMap(value, false);
            if ( actionMap != null )
            {
                nowGameMode = value;
            }
            else
            {
                Debug.LogError( $"�A�N�V�����}�b�v�� '{value}' �� InputActionAsset �ɑ��݂��܂���B" );
            }
        }
    }

    /// <summary>
    /// Awake ���\�b�h�̓I�u�W�F�N�g�̏��������Ɉ�x�Ă΂�܂��B
    /// �V���O���g���̐ݒ�ƑS�A�N�V�����ւ̃R�[���o�b�N�̓o�^���s���܂��B
    /// </summary>
    protected override void AwakeSingleton()
    {
        // �S�ẴA�N�V�����}�b�v�ƃA�N�V�����ɑ΂��ăR�[���o�b�N��o�^���A�L�������܂�
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
    /// �I�u�W�F�N�g���j�������ۂɌĂ΂�܂��B
    /// �o�^�����R�[���o�b�N�̉����ƃA�N�V�����̖��������s���܂��B
    /// </summary>
    protected override void OnDestroySingleton()
    {
        // �S�ẴA�N�V�����}�b�v�ƃA�N�V��������R�[���o�b�N���������A���������܂�
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
    /// �A�N�V�������J�n���ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
    /// ���̓f�[�^��ۑ����܂��B
    /// </summary>
    /// <param name="context">���̓A�N�V�����̃R���e�L�X�g���</param>
    private void OnActionStarted( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// �A�N�V���������s���ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
    /// ���̓f�[�^��ۑ����܂��B
    /// </summary>
    /// <param name="context">���̓A�N�V�����̃R���e�L�X�g���</param>
    private void OnActionPerformed( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// �A�N�V�������L�����Z�����ꂽ�Ƃ��ɌĂяo�����R�[���o�b�N�B
    /// ���̓f�[�^��ۑ����܂��B
    /// </summary>
    /// <param name="context">���̓A�N�V�����̃R���e�L�X�g���</param>
    private void OnActionCanceled( InputAction.CallbackContext context )
    {
        SaveActionData( context );
    }

    /// <summary>
    /// ���̓A�N�V�����̃f�[�^�Ƃ��̃t�F�[�Y��ۑ����܂��B
    /// </summary>
    /// <param name="context">���̓A�N�V�����̃R���e�L�X�g���</param>
    private void SaveActionData( InputAction.CallbackContext context )
    {
        var action = context.action;
        object value = null;

        // �A�N�V�����̊��҂���R���g���[���^�C�v�ɉ����Ēl���擾
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
                // ���̃R���g���[���^�C�v���K�v�ȏꍇ�͂����ɒǉ�
                value = context.ReadValueAsObject();
                break;
        }

        // �A�N�V�����}�b�v���ƃA�N�V��������g�ݍ��킹�ă��j�[�N�ȃL�[���쐬
        string key = $"{action.actionMap.name}/{action.name}";

        // �A�N�V�����f�[�^���X�V�܂��͒ǉ�
        var actionData = new ActionData
        {
            Value = value,
            Phase = action.phase
        };

        actionDataDict[key] = actionData;
    }

    /// <summary>
    /// �w�肵���A�N�V�����}�b�v���ƃA�N�V������������͒l���擾���܂��B
    /// </summary>
    /// <typeparam name="T">�擾�������l�̌^</typeparam>
    /// <param name="actionMapName">�A�N�V�����}�b�v�̖��O</param>
    /// <param name="actionName">�A�N�V�����̖��O</param>
    /// <returns>�w�肵���^�̓��͒l�B���݂��Ȃ��ꍇ�̓f�t�H���g�l�B</returns>
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
    /// �w�肵���A�N�V�����}�b�v���ƃA�N�V����������A�N�V�����̃t�F�[�Y���擾���܂��B
    /// </summary>
    /// <param name="actionMapName">�A�N�V�����}�b�v�̖��O</param>
    /// <param name="actionName">�A�N�V�����̖��O</param>
    /// <returns>�A�N�V�����̌��݂̃t�F�[�Y�B���݂��Ȃ��ꍇ�� Disabled�B</returns>
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
    /// �A�N�V�����̃f�[�^��ێ���������N���X�B
    /// </summary>
    private class ActionData
    {
        /// <summary>
        /// ���͒l��ێ����܂��B�^�̓A�N�V�����̊��҂���R���g���[���^�C�v�Ɉˑ����܂��B
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// �A�N�V�����̌��݂̃t�F�[�Y��ێ����܂��B
        /// </summary>
        public InputActionPhase Phase { get; set; }
    }
}

