using UnityEngine;

public sealed class HeroAutoAttack : MonoBehaviour
{
    [SerializeField] private CombatIdentity combatIdentity;
    [SerializeField] private Health health;
    [SerializeField] private float detectionRange = 4f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private bool rotateToTarget = true;
    [SerializeField] private float rotationSpeed = 12f;

    private Health currentTarget;
    private float attackCooldownTimer;

    private void Awake()
    {
        if (combatIdentity == null)
        {
            combatIdentity = GetComponent<CombatIdentity>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void Update()
    {
        attackCooldownTimer = Mathf.Max(0f, attackCooldownTimer - Time.deltaTime);

        if (health == null || health.IsDead || combatIdentity == null)
        {
            currentTarget = null;
            return;
        }

        currentTarget = FindNearestTarget();

        if (currentTarget == null)
        {
            return;
        }

        if (rotateToTarget)
        {
            RotateTowardsTarget(currentTarget.transform);
        }

        float distanceSqr = (currentTarget.transform.position - transform.position).sqrMagnitude;
        float range = Mathf.Max(0f, attackRange);

        if (distanceSqr <= range * range && attackCooldownTimer <= 0f)
        {
            AttackCurrentTarget();
        }
    }

    private Health FindNearestTarget()
    {
        float range = Mathf.Max(0f, detectionRange);
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            range,
            targetLayers,
            QueryTriggerInteraction.Collide);

        Health nearestTarget = null;
        float nearestDistanceSqr = float.PositiveInfinity;

        foreach (Collider candidateCollider in colliders)
        {
            Health candidateHealth = candidateCollider.GetComponentInParent<Health>();

            if (!IsValidTarget(candidateHealth))
            {
                continue;
            }

            float distanceSqr = (candidateHealth.transform.position - transform.position).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = candidateHealth;
            }
        }

        return nearestTarget;
    }

    private bool IsValidTarget(Health candidateHealth)
    {
        if (candidateHealth == null || candidateHealth == health || candidateHealth.IsDead)
        {
            return false;
        }

        CombatIdentity candidateIdentity = candidateHealth.GetComponent<CombatIdentity>();

        return candidateIdentity != null
            && candidateIdentity != combatIdentity
            && candidateIdentity.IsTargetable
            && candidateIdentity.Team != combatIdentity.Team
            && candidateIdentity.Team == Team.Enemy;
    }

    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    private void AttackCurrentTarget()
    {
        if (!IsValidTarget(currentTarget))
        {
            currentTarget = null;
            return;
        }

        currentTarget.TakeDamage(damage);
        attackCooldownTimer = Mathf.Max(0f, attackCooldown);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, detectionRange));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, attackRange));
    }
}
