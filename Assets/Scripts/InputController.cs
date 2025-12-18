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
    private InputAction randomAttackAction;
    private InputAction heavyAttackAction;

    public Vector2 Movement { get; private set; }
    public bool Jump { get; private set; }
    public bool Attack { get; private set; }
    public bool IsRunning { get; private set; }
    public bool RandomAttack { get; private set; }
    public bool HeavyAttack { get; private set; }

    private void Awake()
    {
        if (playerInputAsset == null)
        {
            Debug.LogError("InputController: playerInputAsset atanmamış!");
            return;
        }

        playerActionMap = playerInputAsset.FindActionMap("Player");
        if (playerActionMap == null)
        {
            Debug.LogError("InputController: 'Player' action map bulunamadı!");
            return;
        }

        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        attackAction = playerActionMap.FindAction("Attack");
        sprintAction = playerActionMap.FindAction("Sprint");
        randomAttackAction = playerActionMap.FindAction("RandomAttack");
        heavyAttackAction = playerActionMap.FindAction("HeavyAttack");

        // Debug: Hangi action'lar bulunamadı?
        if (sprintAction == null) Debug.LogWarning("InputController: 'Sprint' action bulunamadı!");
        if (randomAttackAction == null) Debug.LogWarning("InputController: 'RandomAttack' action bulunamadı!");
        if (heavyAttackAction == null) Debug.LogWarning("InputController: 'HeavyAttack' action bulunamadı!");
    }

    private void OnEnable()
    {
        if (playerActionMap == null) return;
        
        playerActionMap.Enable();

        if (moveAction != null)
        {
            moveAction.performed += OnMovementPerformed;
            moveAction.canceled += OnMovementCanceled;
        }

        if (jumpAction != null)
        {
            jumpAction.performed += OnJumpPerformed;
            jumpAction.canceled += OnJumpCanceled;
        }

        if (attackAction != null)
        {
            attackAction.performed += OnAttackPerformed;
            attackAction.canceled += OnAttackCanceled;
        }

        if (sprintAction != null)
        {
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;
        }

        if (randomAttackAction != null)
        {
            randomAttackAction.performed += OnRandomAttackPerformed;
            randomAttackAction.canceled += OnRandomAttackCanceled;
        }

        if (heavyAttackAction != null)
        {
            heavyAttackAction.performed += OnHeavyAttackPerformed;
            heavyAttackAction.canceled += OnHeavyAttackCanceled;
        }
    }

    private void OnDisable()
    {
        if (playerActionMap == null) return;
        
        playerActionMap.Disable();

        if (moveAction != null)
        {
            moveAction.performed -= OnMovementPerformed;
            moveAction.canceled -= OnMovementCanceled;
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.canceled -= OnJumpCanceled;
        }

        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.canceled -= OnAttackCanceled;
        }

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
        }

        if (randomAttackAction != null)
        {
            randomAttackAction.performed -= OnRandomAttackPerformed;
            randomAttackAction.canceled -= OnRandomAttackCanceled;
        }

        if (heavyAttackAction != null)
        {
            heavyAttackAction.performed -= OnHeavyAttackPerformed;
            heavyAttackAction.canceled -= OnHeavyAttackCanceled;
        }
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
        Debug.Log("Sprint BAŞLADI");
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        IsRunning = false;
        Debug.Log("Sprint DURDU");
    }

    private void LateUpdate()
    {
        Jump = false;
        Attack = false;
        RandomAttack = false;
        HeavyAttack = false;
    }

    private void OnRandomAttackPerformed(InputAction.CallbackContext context)
    {
        RandomAttack = true;
        Debug.Log("RandomAttack (Q) basıldı!");
    }

    private void OnRandomAttackCanceled(InputAction.CallbackContext context)
    {
        RandomAttack = false;
    }

    private void OnHeavyAttackPerformed(InputAction.CallbackContext context)
    {
        HeavyAttack = true;
        Debug.Log("HeavyAttack (E) basıldı!");
    }

    private void OnHeavyAttackCanceled(InputAction.CallbackContext context)
    {
        HeavyAttack = false;
    }
}
