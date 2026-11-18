using GameFramework.Fsm;

public class EnemyStateIdle : FsmState<EnemyLogic>
{
    protected override void OnEnter(IFsm<EnemyLogic> fsm)
    {
        fsm.Owner.StopChase();
    }

    protected override void OnUpdate(IFsm<EnemyLogic> fsm, float elapseSeconds, float realElapseSeconds)
    {
        if (fsm.Owner.IsDead)
        {
            ChangeState<EnemyStateDead>(fsm);
            return;
        }

        if (fsm.Owner.CanAttack)
        {
            ChangeState<EnemyStateAttack>(fsm);
            return;
        }

        if (fsm.Owner.ShouldChase)
            ChangeState<EnemyStateChase>(fsm);
    }
}
