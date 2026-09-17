using GameFramework.Fsm;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

public class Player : EntityLogic
{
    private IFsm<Player> mFsm;
    private Animator animator;
    private FsmComponent fsmComponent;
    private Transform cameraTransform;
    
    private void CreateFsm()
    {
        fsmComponent = GameEntry.GetComponent<FsmComponent>();
        mFsm = fsmComponent.CreateFsm(
            this,
            new PlayerStateIdle(),
            new PlayerStateMove()
        );
    }

    protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
    {
        base.OnAttached(childEntity, parentTransform, userData);
    }

    protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
    {
        base.OnAttachTo(parentEntity, parentTransform, userData);
    }

    protected override void OnDetached(EntityLogic childEntity, object userData)
    {
        base.OnDetached(childEntity, userData);
    }

    protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
    {
        base.OnDetachFrom(parentEntity, userData);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        if (cameraTransform != null && cameraTransform.parent == CachedTransform)
        {
            cameraTransform.SetParent(null, true);
        }

        cameraTransform = null;
        base.OnHide(isShutdown, userData);
    }

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        animator = GetComponentInChildren<Animator>(true);
        return;
    }


    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        CreateFsm();
        mFsm.Start<PlayerStateIdle>();

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
            cameraTransform.SetParent(CachedTransform, false);
            cameraTransform.localPosition = new Vector3(0f, 0f, -10f);
        }
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        UpdateAnimation();
    }

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

    public Vector2 ReadMoveInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float x = (keyboard.dKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed ? 1f : 0f);

        float y = (keyboard.wKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed ? 1f : 0f);

        return new Vector2(x, y).normalized;
    }
}
