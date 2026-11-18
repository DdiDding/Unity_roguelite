using GameFramework.Fsm;

public class EnemyStateDead : FsmState<EnemyLogic>
{
    protected override void OnEnter(IFsm<EnemyLogic> fsm)
    {
        fsm.Owner.BeginDeath();
    }
}
