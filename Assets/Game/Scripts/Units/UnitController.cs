using UnityEngine;

public sealed class UnitController : MonoBehaviour
{
    [SerializeField] private UnitConfig config;
    [SerializeField] private Health health;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private bool stopAtLaneEdge = true;
    [SerializeField] private float leftStopX = -20f;
    [SerializeField] private float rightStopX = 6.5f;

    private float nextAttackTime;

    public UnitConfig Config => config;
    public Team Team => config != null ? config.Team : global::Team.Player;
    public bool IsInitialized { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        EnsureHealthReference();

        if (config != null)
        {
            Initialize(config);
        }
    }

    private void Update()
    {
        if (!IsInitialized || IsDead)
        {
            return;
        }

        Health target = FindNearestTargetInRange();
        if (target != null)
        {
            TryAttack(target);
            return;
        }

        MoveForward();
    }

    private void OnDestroy()
    {
        UnsubscribeFromHealth();
    }

    private void OnDrawGizmosSelected()
    {
        if (config == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, config.AttackRange);
    }

    public void Initialize(UnitConfig unitConfig)
    {
        if (unitConfig == null)
        {
            Debug.LogWarning("UnitController cannot initialize without a UnitConfig.", this);
            return;
        }

        UnsubscribeFromHealth();

        config = unitConfig;
        EnsureHealthReference();

        if (health == null)
        {
            Debug.LogWarning("UnitController cannot initialize without a Health component.", this);
            IsInitialized = false;
            return;
        }

        health.SetMaxHealth(config.MaxHealth, true);
        health.OnDied += HandleDied;

        IsDead = health.IsDead;
        IsInitialized = true;
        nextAttackTime = 0f;
        UpdateVisualFacing();
    }

    private void EnsureHealthReference()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void UnsubscribeFromHealth()
    {
        if (health != null)
        {
            health.OnDied -= HandleDied;
        }
    }

    private Health FindNearestTargetInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, config.AttackRange);

        Health nearestTarget = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            Health candidate = GetValidTargetHealth(hits[i]);
            if (candidate == null)
            {
                continue;
            }

            float distanceSqr = ((Vector2)candidate.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = candidate;
            }
        }

        return nearestTarget;
    }

    private Health GetValidTargetHealth(Collider2D hit)
    {
        if (hit == null)
        {
            return null;
        }

        Health candidateHealth = hit.GetComponentInParent<Health>();
        if (candidateHealth == null || candidateHealth == health || candidateHealth.IsDead)
        {
            return null;
        }

        UnitController unit = candidateHealth.GetComponent<UnitController>();
        if (unit != null)
        {
            return unit != this && unit.IsInitialized && !unit.IsDead && unit.Team != Team
                ? candidateHealth
                : null;
        }

        BaseController baseController = candidateHealth.GetComponent<BaseController>();
        if (baseController != null)
        {
            return baseController.Team != Team ? candidateHealth : null;
        }

        return null;
    }

    private void MoveForward()
    {
        float direction = Team == global::Team.Player ? 1f : -1f;
        if (stopAtLaneEdge && HasReachedStopX(direction))
        {
            return;
        }

        float deltaX = direction * config.MoveSpeed * Time.deltaTime;
        Vector3 position = transform.position;
        position.x += deltaX;

        if (stopAtLaneEdge)
        {
            position.x = Team == global::Team.Player
                ? Mathf.Min(position.x, rightStopX)
                : Mathf.Max(position.x, leftStopX);
        }

        transform.position = position;
    }

    private bool HasReachedStopX(float direction)
    {
        return direction > 0f
            ? transform.position.x >= rightStopX
            : transform.position.x <= leftStopX;
    }

    private void UpdateVisualFacing()
    {
        if (visualRoot == null)
        {
            return;
        }

        Vector3 scale = visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (Team == global::Team.Player ? 1f : -1f);
        visualRoot.localScale = scale;
    }

    private void TryAttack(Health target)
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        target.TakeDamage(config.Damage);
        nextAttackTime = Time.time + config.AttackCooldown;
    }

    private void HandleDied()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        Destroy(gameObject, 0.1f);
    }
}
