using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponAttackRange : MonoBehaviour
{
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Vector2 size = new Vector2(2f, 1f);
    [SerializeField] private Vector2 pivot = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask targetLayers;
    private readonly List<Collider2D> results = new List<Collider2D>();
    private readonly HashSet<StatComponent> hitTargets = new HashSet<StatComponent>();
    private StatComponent ownerStats;

    public void SetOwner(StatComponent owner)
    {
        ownerStats = owner;
    }

    public void OnAttackHit()
    {
        CheckHit(ownerStats);
    }

    private void OnValidate()
    {
        pivot.x = Mathf.Clamp01(pivot.x);
        pivot.y = Mathf.Clamp01(pivot.y);
    }

    private Vector3 GetCenter()
    {
        Vector2 offset = Vector2.Scale(new Vector2(0.5f - pivot.x, 0.5f - pivot.y), size);
        // Keep world-space size while following rotation and mirroring.
        Vector3 right = hitPoint.TransformVector(Vector3.right).normalized;
        Vector3 up = hitPoint.TransformVector(Vector3.up).normalized;
        return hitPoint.position + right * offset.x + up * offset.y;
    }

    private float GetAngle()
    {
        Vector3 right = hitPoint.TransformVector(Vector3.right);
        return Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
    }

    public void CheckHit(StatComponent attacker)
    {
        if (hitPoint == null || attacker == null || attacker.IsDead) return;
        if (size.x <= 0f || size.y <= 0f) return;

        var filter = new ContactFilter2D { useTriggers = true };
        filter.SetLayerMask(targetLayers);
        Physics2D.OverlapBox(GetCenter(), size, GetAngle(), filter, results);
        hitTargets.Clear();

        foreach (Collider2D hit in results)
        {
            if (hit == null) continue;

            StatComponent target = hit.GetComponentInParent<StatComponent>();
            if (target == null || target == attacker || target.IsDead) continue;
            if (!hitTargets.Add(target)) continue;

            target.TakeDamage(attacker.Attack);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Matrix4x4 previous = Gizmos.matrix;
        Color previousColor = Gizmos.color;
        Gizmos.matrix = Matrix4x4.TRS(GetCenter(),
            Quaternion.Euler(0f, 0f, GetAngle()), Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0f));
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitPoint.position, 0.04f);
        Gizmos.matrix = previous;
        Gizmos.color = previousColor;
    }
}
