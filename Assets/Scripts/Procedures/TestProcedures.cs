using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

public class TestProcedures : ProcedureBase
{
    EntityComponent entities;
    protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnDestroy(procedureOwner);
    }

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        Debug.Log("TestProcedures OnEnter");

        entities.ShowEntity<Player>(                   // 로직 타입
            1,                                        // 개체의 고유 ID
            "Assets/Prefabs/Player.prefab",  // 프리팹 에셋 경로
            "Player",                                 // 소속 그룹 이름
            Vector3.zero                              // 로직에 전달할 초기 데이터
        );
    }

    protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnInit(procedureOwner);
        entities = GameEntry.GetComponent<EntityComponent>();
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }
}
