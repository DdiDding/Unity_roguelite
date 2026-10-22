using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TestBullet : MonoBehaviour
{
    [Min(0f)] public float speed = 5f;
    [SerializeField, Min(0.001f)] private float radius = 0.1f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;
    [SerializeField] private LayerMask hitLayers = Physics2D.DefaultRaycastLayers;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();
    private Rigidbody2D body;
    private StatComponent owner;
    private int attack;
    private ContactFilter2D filter;
    private bool initialized;
    private bool consumed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Init(StatComponent ownerStats)
    {
        if (ownerStats == null || ownerStats.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        owner = ownerStats;
        attack = ownerStats.Attack;
        filter = new ContactFilter2D { useTriggers = true };
        filter.SetLayerMask(hitLayers);
        initialized = true;
    }

    private void Start()
    {
        if (!initialized)
        {
            Destroy(gameObject);
            return;
        }

        body.gravityScale = 0f;
        body.linearVelocity = (Vector2)transform.right * Mathf.Max(0f, speed);
        Destroy(gameObject, Mathf.Max(0.01f, lifetime));
    }

    private void FixedUpdate()
    {
        if (!initialized || consumed) return;

        Vector2 velocity = body.linearVelocity;
        float distance = velocity.magnitude * Time.fixedDeltaTime;
        if (distance <= 0f) return;

        Physics2D.CircleCast(body.position, Mathf.Max(0.001f, radius),
            velocity.normalized, filter, hits, distance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.attachedRigidbody == body) continue;

            StatComponent target = hit.collider.GetComponentInParent<StatComponent>();
            if (target != null && (target == owner || target.IsDead)) continue;

            consumed = true;
            body.linearVelocity = Vector2.zero;
            body.position = hit.centroid;
            if (target != null) target.TakeDamage(attack);
            Destroy(gameObject);
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color previous = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.001f, radius));
        Gizmos.color = previous;
    }
}
