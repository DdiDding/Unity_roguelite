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
    private Transform aimTransform;

    public WeaponLogic CurrentWeapon { get; private set; }




    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////
    private void OnRequestEquipSuccess(object sender, GameEventArgs args)
    {
        // TODO: 유효성 검사
        var weaponEntity = (ShowEntitySuccessEventArgs)args;

        if (!weaponId.HasValue || weaponEntity.Entity.Id != weaponId.Value) return;

        entitiyComponent.AttachEntity(weaponEntity.Entity, owner.Entity, owerSocket);
        WeaponLogic weapon = (WeaponLogic)weaponEntity.Entity.Logic;

        weapon.CachedTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        weapon.CachedTransform.localScale = Vector3.one;

        CurrentWeapon = weapon;
    }

    private void OnRequestEquipFailure(object sender, GameEventArgs args)
    {
        Debug.LogError("WeaponComponent: OnRequestEquipFailure");
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void CustomInit(EntityLogic owner, Transform owerSocket)
    {
        this.owner = owner;
        this.owerSocket = owerSocket;
        this.aimTransform = transform.Find("Aim");

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
