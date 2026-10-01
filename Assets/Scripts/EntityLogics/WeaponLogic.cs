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

        this.aimPivot = CachedTransform.Find("AttackPivot/AimPivot");

        return;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        UpdateAimRotation();
    }

    private void UpdateAimRotation(Vector2 worldPosition)
    {
        Vector2 direction = worldPosition - (Vector2)aimPivot.position;

        bool isZeroDirection = direction.sqrMagnitude < 0.0001f;
        if (isZeroDirection == true) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetAimTarget(Vector2 worldPosition)
    {
        aimTargetPosition = worldPosition;
    }
}
