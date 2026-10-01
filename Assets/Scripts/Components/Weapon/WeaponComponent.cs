using GameFramework.Event;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityGameFramework.Runtime;

public class WeaponComponent : MonoBehaviour
{
    private readonly EntityComponent entitiyComponent = GameEntry.GetComponent<EntityComponent>();
    private readonly EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();

    private EntityLogic owner;
    private Transform owerSocket;

    private int? weaponId;

    private Transform aimPivot;
    public WeaponLogic EquipWeapon { get; private set; }


    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnRequestEquipSuccess(object sender, GameEventArgs args)
    {
        var weaponEntity = (ShowEntitySuccessEventArgs)args;
        if (!weaponId.HasValue || weaponEntity.Entity.Id != weaponId.Value) return;

        entitiyComponent.AttachEntity(weaponEntity.Entity, owner.Entity, owerSocket);

        this.EquipWeapon = (WeaponLogic)weaponEntity.Entity.Logic;
        this.EquipWeapon.CachedTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        this.EquipWeapon.CachedTransform.localScale = Vector3.one;

        this.aimPivot = EquipWeapon.CachedTransform.Find("AttackPivot/AimPivot");
    }

    private void OnRequestEquipFailure(object sender, GameEventArgs args)
    {
        Debug.LogError("WeaponComponent: OnRequestEquipFailure");
    }

    // 마우스에 따라 Weapon의 AimPivot를 회전
    // Player와 Enemy에 따라 처리 가능하게끔 매개변수로 받음
    private void SetAimTarget(Vector2 worldPosition)
    {
        EquipWeapon.SetAimTarget(worldPosition);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    // 무기가 없을 수도 있다는 것에 주의
    public void CustomInit(EntityLogic owner, Transform owerSocket)
    {
        this.owner = owner;
        this.owerSocket = owerSocket;

        // Subscribe to events
        eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnRequestEquipSuccess);
        eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnRequestEquipFailure);
    }

    /* RequestEquip
     * 장착하려는 WeaponEntity의 Show요청
     */
    public void RequestEquip(int requestEntityId, string asset)
    {
        // TODO : Unequip();

        this.weaponId = requestEntityId;
        entitiyComponent.ShowEntity<WeaponLogic>(requestEntityId, asset, "Weapon", (object) requestEntityId);
    }

    public void DoNormalAttack()
    {

    }
}
