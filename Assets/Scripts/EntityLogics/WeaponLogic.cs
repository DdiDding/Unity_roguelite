using UnityEngine;
using UnityGameFramework.Runtime;

public class WeaponLogic :EntityLogic
{
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private property
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private Transform aimPivot;
    private Vector2 aimTargetPosition;

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Life Cycle
    //////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);

        this.aimPivot = CachedTransform.Find("WeaponRoot/AttackPivot/AimPivot");

        return;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        UpdateAimRotation();
    }

    private void UpdateAimRotation()
    {
        if (aimPivot == null)
            return;

        Vector2 worldDirection =
            aimTargetPosition - (Vector2)aimPivot.position;

        // 부모의 좌우 반전까지 반영하여 무기 루트 기준으로 변환
        Vector2 localDirection = CachedTransform.InverseTransformVector(worldDirection);

        bool isNearZero = localDirection.sqrMagnitude < 0.0001f;
        if (isNearZero) return;

        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        aimPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetAimTarget(Vector2 worldPosition)
    {
        aimTargetPosition = worldPosition;
    }
}
