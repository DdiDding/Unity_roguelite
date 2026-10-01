using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;
using GameFramework.Fsm;

public class PlayerLogic : EntityLogic
{
    private IFsm<PlayerLogic> mFsm;
    private Animator animator;
    private Transform cameraTransform;

    private InputComponent inputComponent { get; set; }
    private FsmComponent fsmComponent;
    private WeaponComponent weaponComponent;

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Life Cycle
    //////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);

        inputComponent = GameEntry.GetComponent<InputComponent>();

        weaponComponent = GetComponent<WeaponComponent>();
        Transform upperSocket = CachedTransform.Find("Visual/WeaponSocketUpper");
        weaponComponent?.CustomInit(this, upperSocket);
        animator = GetComponentInChildren<Animator>(true);

        return;
    }

  
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);

        // FSM Setting
        {
            CreateFsm();
            mFsm.Start<PlayerStateIdle>();
        }

        // Camera Setting
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                cameraTransform.SetParent(CachedTransform, false);
                cameraTransform.localPosition = new Vector3(0f, 0f, -10f);
            }
        }

        // Event Subscription (Hide때 구독 해지 해야하므로 Show에서 구독 설정)
        {
            inputComponent.OnAttackEvent += OnAttack;
        }
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        UpdateAnimation();
        UpdateWeaponAim()

        TestKey();
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        // FSM hide
        fsmComponent.DestroyFsm(mFsm);

        // Camera hide
        {
            if (cameraTransform != null && cameraTransform.parent == CachedTransform)
            {
                cameraTransform.SetParent(null, true);
            }
            cameraTransform = null;
        }


        // Event Unsubscription
        {
            inputComponent.OnAttackEvent -= OnAttack;
        }

        base.OnHide(isShutdown, userData);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Function
    //////////////////////////////////////////////////////////////////////////////////////////////////

    // Anim관련 값이 많아질거 같아서 일단 함수로 묶어둠
    private void UpdateAnimation()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool isLookUp = mousePos.y > Screen.height * 0.5f;

        animator.SetBool("isLookUp", isLookUp);

        bool faceLeft = mousePos.x < Screen.width * 0.5f;
        CachedTransform.Find("Visual").SetLocalScaleX((faceLeft ? -1f : 1f));

        bool isMove = mFsm != null && mFsm.CurrentState is PlayerStateMove;
        animator.SetBool("isMove", isMove);
    }


    private void UpdateWeaponAim()
    {
        if (weaponComponent.EquipWeapon == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        weaponComponent.SetAimTarget(worldMousePos);
    }

    private void CreateFsm()
    {
        fsmComponent = GameEntry.GetComponent<FsmComponent>();
        mFsm = fsmComponent.CreateFsm(
            this,
            new PlayerStateIdle(),
            new PlayerStateMove()
        );
    }


    private void OnAttack()
    {
        // DoSomething when attack input is performed
        Debug.Log("PlayerLogic: Attack input performed!");
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Function
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public Vector2 ReadMoveInput()
    {
        return inputComponent.ReadMoveInput();
    }

    // 나중에 키 테스트할지도 모름
    public void TestKey()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame == true)
        {
            Debug.Log("PlayerLogic: Q key pressed - Requesting to equip GreatSword.");
            weaponComponent.RequestEquip(11, "Assets/Prefabs/Weapons/GreatSword_Basic.prefab");
        }
    }
}
