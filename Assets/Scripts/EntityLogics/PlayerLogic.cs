using GameFramework.Fsm;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

public class PlayerLogic : EntityLogic
{
    private bool bIsLookUp;
    bool bIsfaceLeft;
    bool bIsMove;

    private IFsm<PlayerLogic> mFsm;
    private Animator animator;
    private Transform upperSocket;
    private Transform lowerSocket;
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

        animator = GetComponentInChildren<Animator>(true);
        lowerSocket = CachedTransform.Find("Visual/WeaponSocketLower");
        upperSocket = CachedTransform.Find("Visual/WeaponSocketUpper");

        inputComponent = GameEntry.GetComponent<InputComponent>();
        weaponComponent = GetComponent<WeaponComponent>();
        weaponComponent?.CustomInit(this, upperSocket); //TODO : Equip 분리

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
        UpdateData();
        UpdateAnimation();
        UpdateWeapon();

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

    private void UpdateData()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bIsLookUp = mousePos.y > Screen.height * 0.5f;

        bIsfaceLeft = mousePos.x < Screen.width * 0.5f;

        bIsMove = mFsm != null && mFsm.CurrentState is PlayerStateMove;
    }

    // Anim관련 값이 많아질거 같아서 일단 함수로 묶어둠
    private void UpdateAnimation()
    {
        animator.SetBool("isLookUp", bIsLookUp);

        CachedTransform.Find("Visual").SetLocalScaleX((bIsfaceLeft ? -1f : 1f));

        animator.SetBool("isMove", bIsMove);
    }


    private void UpdateWeapon()
    {
        // Update socket
        {
            weaponComponent.SetSocket(bIsLookUp ? upperSocket : lowerSocket);
        }

        // Update WeaponAim
        {
            if (weaponComponent.EquipWeapon == null) return;

            Vector3 screenPoint = Mouse.current.position.ReadValue();
            screenPoint.z = Camera.main.WorldToScreenPoint(CachedTransform.position).z;

            Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(screenPoint);
            weaponComponent.SetAimTarget(worldMousePos);
        }
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
