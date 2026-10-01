using UnityEngine;
using UnityGameFramework.Runtime;

public class WeaponLogic :EntityLogic
{
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private property
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private Transform aimPivot;
    private Vector2 aimTargetPosition;

    private Animator animator;

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Life Cycle
    //////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);

        this.aimPivot = CachedTransform.Find("WeaponRoot/AttackPivot/AimPivot");
        this.animator = GetComponent<Animator>();

        return;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        UpdateAimRotation();
    }

    private void UpdateAimRotation()
    {
        if (aimPivot == null) return;


        Vector2 targetDirection = aimTargetPosition - (Vector2)aimPivot.position;

        // World pos => LocalPos
        Vector2 localDirection = CachedTransform.InverseTransformVector(targetDirection);

        bool isNearZero = localDirection.sqrMagnitude < 0.0001f;
        if (isNearZero) return;

        // target의 각도 구하기
        // Atan2 : 방향 => 각도
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        aimPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void DoNormalAttak()
    {
        if (animator == null) return;

        bool isStateIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        if (isStateIdle == false) return;

        animator.SetTrigger("Attak");
    }

    public void SetAimTarget(Vector2 worldPosition)
    {
        aimTargetPosition = worldPosition;
    }

}
