using GameFramework.Fsm;
using UnityEngine;
using UnityEngine.AI;
using UnityGameFramework.Runtime;

public class EnemyLogic : EntityLogic
{
    private const float AttackRange = 1.2f;
    private const float AttackInterval = 1f;
    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D hitCollider;
    private StatComponent stats;
    private Transform target;
    private StatComponent targetStats;
    private FsmComponent fsmComponent;
    private IFsm<EnemyLogic> fsm;
    private float pathUpdateRemaining;
    private float attackCooldownRemaining;
    private bool attackInProgress;
    private bool attackHitConsumed;
    private bool attackFinished;
    private static readonly int IsMove = Animator.StringToHash("isMove");

    public bool IsDead => stats.IsDead;

    private bool HasLivingTarget => agent.isActiveAndEnabled && agent.isOnNavMesh && !stats.IsDead
        && target != null && target.gameObject.activeInHierarchy && targetStats != null
        && !targetStats.IsDead;

    private bool IsInAttackRange => ((Vector2)(target.position - CachedTransform.position)).sqrMagnitude
        <= AttackRange * AttackRange;

    public bool CanAttack => HasLivingTarget && attackCooldownRemaining <= 0f
        && IsInAttackRange && !agent.Raycast(target.position, out _);

    public bool AttackFinished => attackFinished || !HasLivingTarget;

    public bool ShouldChase => HasLivingTarget
        && ((Vector2)(target.position - CachedTransform.position)).sqrMagnitude
            > (agent.stoppingDistance + 0.05f) * (agent.stoppingDistance + 0.05f);

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = CachedTransform.Find("Visual").GetComponent<SpriteRenderer>();
        hitCollider = GetComponent<Collider2D>();
        stats = GetComponent<StatComponent>();
        fsmComponent = GameEntry.GetComponent<FsmComponent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        stats.ResetHealth();
        hitCollider.enabled = true;
        target = userData as Transform;
        targetStats = target != null ? target.GetComponent<StatComponent>() : null;
        pathUpdateRemaining = 0f;
        attackCooldownRemaining = 0f;
        attackInProgress = false;
        attackHitConsumed = false;
        attackFinished = false;
        spriteRenderer.flipX = false;
        animator.SetBool(IsMove, false);
        animator.Play("Idle", 0, 0f);

        if (target == null)
        {
            Debug.LogError("Mole needs a player target.", this);
            return;
        }

        var filter = new NavMeshQueryFilter
        {
            agentTypeID = agent.agentTypeID,
            areaMask = agent.areaMask
        };
        var path = new NavMeshPath();
        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = Quaternion.Euler(0f, 0f, i * 90f) * Vector3.right * 3f;
            if (!NavMesh.SamplePosition(target.position + offset, out NavMeshHit hit, 2f, filter) ||
                !NavMesh.CalculatePath(hit.position, target.position, filter, path) ||
                path.status != NavMeshPathStatus.PathComplete)
                continue;

            CachedTransform.SetPositionAndRotation(hit.position, Quaternion.identity);
            agent.enabled = true;
            Debug.Assert(agent.isOnNavMesh, "Mole must spawn on its NavMesh.", this);
            fsm = fsmComponent.CreateFsm(Entity.Id.ToString(), this,
                new EnemyStateIdle(), new EnemyStateChase(), new EnemyStateAttack(), new EnemyStateDead());
            fsm.Start<EnemyStateIdle>();
            return;
        }

        Debug.LogError("No reachable NavMesh spawn point near the player. Check the baked NavMesh.", this);
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        attackCooldownRemaining = Mathf.Max(0f, attackCooldownRemaining - elapseSeconds);
        bool isMoving = !attackInProgress && agent.isActiveAndEnabled && agent.isOnNavMesh
            && !agent.isStopped && agent.velocity.sqrMagnitude > 0.0001f;
        animator.SetBool(IsMove, isMoving);
        if (isMoving && Mathf.Abs(agent.velocity.x) > 0.01f)
            spriteRenderer.flipX = agent.velocity.x < 0f;
    }

    public void BeginChase()
    {
        agent.isStopped = false;
        pathUpdateRemaining = 0f;
    }

    public void UpdateChase(float elapseSeconds)
    {
        pathUpdateRemaining -= elapseSeconds;
        if (pathUpdateRemaining > 0f)
            return;

        pathUpdateRemaining = 0.2f;
        agent.SetDestination(target.position);
    }

    public void StopChase()
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        agent.ResetPath();
        agent.isStopped = true;
    }

    public void BeginAttack()
    {
        StopChase();
        attackInProgress = true;
        attackHitConsumed = false;
        attackFinished = false;
        float directionX = target.position.x - CachedTransform.position.x;
        if (Mathf.Abs(directionX) > 0.01f)
            spriteRenderer.flipX = directionX < 0f;
        animator.SetBool(IsMove, false);
        animator.Play("Attack", 0, 0f);
    }

    public void OnAttackHit()
    {
        if (!attackInProgress || attackHitConsumed || attackFinished
            || !(fsm?.CurrentState is EnemyStateAttack))
            return;

        attackHitConsumed = true;
        if (HasLivingTarget && IsInAttackRange && !agent.Raycast(target.position, out _))
            targetStats.TakeDamage(stats.Attack);
    }

    public void OnAttackEnd()
    {
        if (attackInProgress && fsm?.CurrentState is EnemyStateAttack)
            attackFinished = true;
    }

    public void EndAttack()
    {
        attackInProgress = false;
        attackHitConsumed = false;
        attackFinished = false;
        attackCooldownRemaining = AttackInterval;
        animator.Play("Idle", 0, 0f);
    }

    public void BeginDeath()
    {
        StopChase();
        agent.enabled = false;
        hitCollider.enabled = false;
        attackInProgress = false;
        animator.SetBool(IsMove, false);
        animator.Play("Down", 0, 0f);
    }

    public void OnDeathEnd()
    {
        if (IsDead && fsm?.CurrentState is EnemyStateDead)
            GameEntry.GetComponent<EntityComponent>().HideEntity(Entity.Id);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        if (fsm != null)
        {
            fsmComponent.DestroyFsm(fsm);
            fsm = null;
        }
        agent.enabled = false;
        attackInProgress = false;
        attackCooldownRemaining = 0f;
        target = null;
        targetStats = null;
        base.OnHide(isShutdown, userData);
    }
}
