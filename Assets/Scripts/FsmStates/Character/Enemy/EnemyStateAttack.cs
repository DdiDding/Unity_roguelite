using GameFramework.Fsm;

public class EnemyStateAttack : FsmState<EnemyLogic>
{
    protected override void OnEnter(IFsm<EnemyLogic> fsm)
    {
        fsm.Owner.BeginAttack();
    }

    protected override void OnUpdate(IFsm<EnemyLogic> fsm, float elapseSeconds, float realElapseSeconds)
    {
        if (fsm.Owner.IsDead)
        {
            ChangeState<EnemyStateDead>(fsm);
            return;
        }

        if (!fsm.Owner.AttackFinished)
            return;

        if (fsm.Owner.ShouldChase)
            ChangeState<EnemyStateChase>(fsm);
        else
            ChangeState<EnemyStateIdle>(fsm);
    }

    protected override void OnLeave(IFsm<EnemyLogic> fsm, bool isShutdown)
    {
        fsm.Owner.EndAttack();
    }
}
