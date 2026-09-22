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

public class PlayerStateIdle : FsmState<Player>
{
    protected override void OnUpdate(
       IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {
        Debug.Log("PlayerStateIdle OnUpdate");
        if (fsm.Owner.ReadMoveInput() != Vector2.zero)
            ChangeState<PlayerStateMove>(fsm);
    }
}
