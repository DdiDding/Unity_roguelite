using GameFramework.Fsm;
using UnityEngine;
using UnityGameFramework.Runtime;

enum PlayerState
{
    Idle,
    Move,
    Attack,
    Jump,
    Die
}

public class PlayerStateIdle : FsmState<PlayerLogic>
{
    protected override void OnUpdate(
       IFsm<PlayerLogic> fsm, float elapseSeconds, float realElapseSeconds)
    {
        if (fsm.Owner.ReadMoveInput() != Vector2.zero)
            ChangeState<PlayerStateMove>(fsm);
    }
}
