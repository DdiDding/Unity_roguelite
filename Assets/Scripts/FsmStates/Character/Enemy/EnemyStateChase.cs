using GameFramework.Fsm;

public class EnemyStateChase : FsmState<EnemyLogic>
{
    protected override void OnEnter(IFsm<EnemyLogic> fsm)
    {
        fsm.Owner.BeginChase();
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

        if (!fsm.Owner.ShouldChase)
        {
            ChangeState<EnemyStateIdle>(fsm);
            return;
        }

        fsm.Owner.UpdateChase(elapseSeconds);
    }
}
