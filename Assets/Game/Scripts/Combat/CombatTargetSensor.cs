using UnityEngine;

public sealed class CombatTargetSensor : MonoBehaviour
{
    [SerializeField] private CombatIdentity ownerIdentity;
    [SerializeField] private float detectionRange = 4f;
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private bool requireDifferentTeam = true;
    [SerializeField] private bool ignoreSelf = true;

    public float DetectionRange => detectionRange;

    private void Awake()
    {
        if (ownerIdentity == null)
        {
            ownerIdentity = GetComponent<CombatIdentity>();
        }
    }

    public bool TryFindNearestTarget(out Health targetHealth)
    {
        return TryFindNearestTarget(out targetHealth, out _);
    }

    public bool TryFindNearestTarget(
        out Health targetHealth,
        out CombatIdentity targetIdentity)
    {
        targetHealth = null;
        targetIdentity = null;

        if (requireDifferentTeam && ownerIdentity == null)
        {
            return false;
        }

        float range = Mathf.Max(0f, detectionRange);
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            range,
            targetLayers,
            QueryTriggerInteraction.Collide);

        float nearestDistanceSqr = float.PositiveInfinity;

        foreach (Collider candidateCollider in colliders)
        {
            Health candidateHealth = candidateCollider.GetComponentInParent<Health>();
            CombatIdentity candidateIdentity = candidateCollider.GetComponentInParent<CombatIdentity>();

            if (!IsValidTarget(candidateCollider, candidateHealth, candidateIdentity))
            {
                continue;
            }

            Vector3 offset = candidateHealth.transform.position - transform.position;
            float distanceSqr = offset.x * offset.x + offset.z * offset.z;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                targetHealth = candidateHealth;
                targetIdentity = candidateIdentity;
            }
        }

        return targetHealth != null;
    }

    private bool IsValidTarget(
        Collider candidateCollider,
        Health candidateHealth,
        CombatIdentity candidateIdentity)
    {
        if (candidateHealth == null
            || candidateHealth.IsDead
            || candidateIdentity == null
            || !candidateIdentity.IsTargetable)
        {
            return false;
        }

        if (ignoreSelf && IsOwnedBySelf(candidateCollider, candidateIdentity))
        {
            return false;
        }

        return !requireDifferentTeam || candidateIdentity.Team != ownerIdentity.Team;
    }

    private bool IsOwnedBySelf(Collider candidateCollider, CombatIdentity candidateIdentity)
    {
        if (ownerIdentity != null && candidateIdentity == ownerIdentity)
        {
            return true;
        }

        Transform candidateTransform = candidateCollider.transform;
        return candidateTransform == transform
            || candidateTransform.IsChildOf(transform)
            || transform.IsChildOf(candidateTransform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(0f, detectionRange));
    }
}
