using GameFramework.Fsm;
using UnityEngine;

public class PlayerStateMove : FsmState<Player>
{
    protected override void OnUpdate(IFsm<Player> fsm, float elapseSeconds, float realElapseSeconds)
    {

        Vector2 direction = fsm.Owner.ReadMoveInput();

        Debug.Log("PlayerStateMove OnUpdate" + direction);

        if (direction == Vector2.zero)
        {
            ChangeState<PlayerStateIdle>(fsm);
            return;
        }

        const float moveSpeed = 5f;

        fsm.Owner.CachedTransform.position += (Vector3)direction * moveSpeed * elapseSeconds;
    }
}
