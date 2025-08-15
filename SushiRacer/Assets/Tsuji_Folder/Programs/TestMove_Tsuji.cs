using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove_Tsuji : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.TestAction.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.TestAction.Move.canceled += ctx => moveInput = Vector2.zero;
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
        transform.Translate(move * 10f * Time.deltaTime);
    }
}