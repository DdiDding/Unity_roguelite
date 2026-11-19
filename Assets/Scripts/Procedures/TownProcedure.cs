using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Event;
using NavMeshPlus.Components;
using UnityEngine;
using UnityGameFramework.Runtime;

public class TownProcedure : ProcedureBase
{
    EntityComponent entities;
    private EventComponent events;
    protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnDestroy(procedureOwner);
    }

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("TownProcedure OnEnter");

        events.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        entities.AddEntityGroup("Enemy", 60f, 16, 60f, 0);

        var surface = Object.FindFirstObjectByType<NavMeshSurface>();
        if (surface != null && surface.navMeshData == null)
            surface.BuildNavMesh();

        entities.ShowEntity<PlayerLogic>(                   // 로직 타입
            1,                                        // 개체의 고유 ID
            "Assets/Prefabs/Characters/Player.prefab",  // 프리팹 에셋 경로
            "Player",                                 // 소속 그룹 이름
            Vector3.zero                              // 로직에 전달할 초기 데이터
        );
    }

    protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnInit(procedureOwner);
        entities = GameEntry.GetComponent<EntityComponent>();
        events = GameEntry.GetComponent<EventComponent>();
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        events.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        if (entities.HasEntity(2) || entities.IsLoadingEntity(2))
            entities.HideEntity(2);

        base.OnLeave(procedureOwner, isShutdown);
    }

    private void OnShowEntitySuccess(object sender, GameEventArgs args)
    {
        var shown = (ShowEntitySuccessEventArgs)args;
        if (shown.Entity.Id != 1)
            return;

        entities.ShowEntity<EnemyLogic>(2,
            "Assets/Prefabs/Characters/Mole.prefab",
            "Enemy", shown.Entity.Logic.CachedTransform);
    }

    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }
}
