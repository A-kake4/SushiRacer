using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove_Tsuji : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    [SerializeField]
    private RacerProgress_Tsuji racerProgress;

    [SerializeField]
    private SequentialOperation_Tsuji so;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.TestAction.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.TestAction.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.TestAction.Warp.performed += ctx => OnWarp();
        controls.TestAction.Change30.performed += ctx => Change30();
        controls.TestAction.Change50.performed += ctx => Change50();
        controls.TestAction.Change100.performed += ctx => Change100();
    }

    private void OnEnable()
    {
        controls.TestAction.Enable();
    }

    private void OnDisable()
    {
        controls.TestAction.Disable();
    }

    private void Update()
    {
        // ó·: íPèÉÇ…TranslateÇ≈ìÆÇ©Ç∑
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(move * 30f * Time.deltaTime);
    }

    void OnWarp()
    {
        racerProgress.Teleport();

    }

    void Change30()
    {
        so.SetPasteSeal(SealPercent.Percent_30);

    }

    void Change50()
    {
        so.SetPasteSeal(SealPercent.Percent_50);

    }
    
    void Change100()
    {
        so.SetPasteSeal(SealPercent.None);

    }
}