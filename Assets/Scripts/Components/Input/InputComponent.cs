using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

[DisallowMultipleComponent] // 이 컴포넌트가 GameObject에 여러 번 추가되는 것을 방지
public sealed class InputComponent : GameFrameworkComponent
{
    private InputAction moveAction;
    private InputAction attackAction;
    
    public event System.Action OnAttackEvent;

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Life Cycle
    //////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start() // MonoBehaviour에 정의하는 Start 메서드, 게임 시작 시 한 번 호출
    {
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
        attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        attackAction.performed += OnLeftClick;
    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        attackAction.performed -= OnLeftClick;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        OnAttackEvent?.Invoke();

        return;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public Vector2 ReadMoveInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        return Vector2.ClampMagnitude(input, 1f);
    }
}
