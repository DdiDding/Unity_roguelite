using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Playables;

public class WeaponLogic :EntityLogic
{
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private property
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private Transform aimPivot;
    private Vector2 aimTargetPosition;

    private Animator animator;
    private WeaponAttackRange attackRange;
    private LaunchProjectile projectileLauncher;

    private SkillConfig skillConfig;
    private PlayableGraph animationGraph;
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Life Cycle
    //////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);

        this.aimPivot = CachedTransform.Find("WeaponRoot/AttackPivot/AimPivot");
        this.animator = GetComponent<Animator>();
        this.attackRange = GetComponent<WeaponAttackRange>();
        this.projectileLauncher = GetComponentInChildren<LaunchProjectile>(true);
        this.skillConfig = GetComponentInChildren<SkillConfig>();

        return;
    }

    protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
    {
        base.OnAttachTo(parentEntity, parentTransform, userData);
        StatComponent ownerStats = parentEntity.GetComponent<StatComponent>();
        if (attackRange != null)
        {
            attackRange.SetOwner(ownerStats);
        }
        if (projectileLauncher != null)
        {
            projectileLauncher.SetOwner(ownerStats);
        }
    }

    protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
    {
        if (attackRange != null)
        {
            attackRange.SetOwner(null);
        }
        if (projectileLauncher != null)
        {
            projectileLauncher.SetOwner(null);
        }
        base.OnDetachFrom(parentEntity, userData);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        if (attackRange != null)
        {
            attackRange.SetOwner(null);
        }
        if (projectileLauncher != null)
        {
            projectileLauncher.SetOwner(null);
        }
        StopClip();
        base.OnHide(isShutdown, userData);
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        UpdateAimRotation();

        // 공격 애니메이션 끝나면 기존 animator로 돌아가기
        if (animationGraph.IsValid() && animationGraph.IsDone())
        {
            StopClip();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateAimRotation()
    {
        if (aimPivot == null) return;


        Vector2 targetDirection = aimTargetPosition - (Vector2)aimPivot.position;

        // World pos => LocalPos
        Vector2 localDirection = CachedTransform.InverseTransformVector(targetDirection);

        bool isNearZero = localDirection.sqrMagnitude < 0.0001f;
        if (isNearZero) return;

        // target의 각도 구하기
        // Atan2 : 방향 => 각도
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        aimPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void StopClip()
    {
        if (animationGraph.IsValid())
        {
            animationGraph.Stop();
            animationGraph.Destroy();
        };
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Public Functions
    //////////////////////////////////////////////////////////////////////////////////////////////////

    public void DoNormalAttak()
    {
        if (animator == null) return;
        if (skillConfig == null) return;


        AnimationClip clip = skillConfig.NormalAttak.GetSkill(0).AnimationClip;

        StopClip();
        var playable = AnimationPlayableUtilities.PlayClip(this.animator, clip, out animationGraph);
        playable.SetDuration(clip.length);

        //bool isStateIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        //if (isStateIdle == false) return;
        //animator.SetTrigger("Attak");
    }
    public void SetAimTarget(Vector2 worldPosition)
    {
        aimTargetPosition = worldPosition;
    }

    

}
