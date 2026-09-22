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

    private InputComponent inputComponent { get; set; }
    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        animator = GetComponentInChildren<Animator>(true);

        inputComponent = GameEntry.GetComponent<InputComponent>();

        return;
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

    protected override void OnHide(bool isShutdown, object userData)
    {
        if (cameraTransform != null && cameraTransform.parent == CachedTransform)
        {
            cameraTransform.SetParent(null, true);
        }
        cameraTransform = null;

        fsmComponent.DestroyFsm(mFsm);

        base.OnHide(isShutdown, userData);
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
        return inputComponent.ReadMoveInput();
    }

    // 나중에 키 테스트할지도 모름
    //public TestKey()
    //{
    //    var keyboard = Keyboard.current;
    //    if (keyboard == null)
    //        return Vector2.zero;

    //    float x = (keyboard.dKey.isPressed ? 1f : 0f)
    //            - (keyboard.aKey.isPressed ? 1f : 0f);
    //}
}
