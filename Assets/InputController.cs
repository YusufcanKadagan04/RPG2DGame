using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset playerInputAsset;
    
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private InputAction sprintAction;

    public Vector2 Movement { get; private set; }
    public bool Jump { get; private set; }
    public bool Attack { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        playerActionMap = playerInputAsset.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        attackAction = playerActionMap.FindAction("Attack");
        sprintAction = playerActionMap.FindAction("Sprint");
    }

    private void OnEnable()
    {
        playerActionMap.Enable();

        moveAction.performed += OnMovementPerformed;
        moveAction.canceled += OnMovementCanceled;

        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;

        attackAction.performed += OnAttackPerformed;
        attackAction.canceled += OnAttackCanceled;

        sprintAction.performed += OnSprintPerformed;
        sprintAction.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        playerActionMap.Disable();

        moveAction.performed -= OnMovementPerformed;
        moveAction.canceled -= OnMovementCanceled;

        jumpAction.performed -= OnJumpPerformed;
        jumpAction.canceled -= OnJumpCanceled;

        attackAction.performed -= OnAttackPerformed;
        attackAction.canceled -= OnAttackCanceled;

        sprintAction.performed -= OnSprintPerformed;
        sprintAction.canceled -= OnSprintCanceled;
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        Movement = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Jump = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        Jump = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Attack = true;
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        Attack = false;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        IsRunning = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        IsRunning = false;
    }

    private void LateUpdate()
    {
        Jump = false;
        Attack = false;
    }
}