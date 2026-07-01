using System;
using UnityEngine;

public sealed class EnemySquadController : MonoBehaviour
{
    [SerializeField] private EnemySquadConfig config;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Health health;
    [SerializeField] private CombatIdentity combatIdentity;
    [SerializeField] private CombatTargetSensor targetSensor;
    [SerializeField] private Health fallbackTarget;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float destroyDelayAfterDeath = 0.2f;

    private Health currentTarget;
    private float attackCooldownTimer;
    private float verticalVelocity;
    private bool isDead;
    private bool isSubscribedToHealth;
    private bool hasLoggedMissingController;

    public event Action<EnemySquadController> OnSquadDied;

    public EnemySquadConfig Config => config;
    public Health Health => health;
    public bool IsInitialized { get; private set; }
    public bool IsDead => isDead;
    public Health CurrentTarget => currentTarget;

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if (combatIdentity == null)
        {
            combatIdentity = GetComponent<CombatIdentity>();
        }

        if (targetSensor == null)
        {
            targetSensor = GetComponent<CombatTargetSensor>();
        }

        if (characterController == null)
        {
            LogMissingControllerWarning();
        }

        if (config != null)
        {
            Initialize(config, fallbackTarget);
        }
    }

    private void Update()
    {
        if (!IsInitialized || isDead)
        {
            return;
        }

        attackCooldownTimer = Mathf.Max(0f, attackCooldownTimer - Time.deltaTime);
        currentTarget = FindTarget();

        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = null;
            return;
        }

        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0f;
        float distanceSqr = direction.sqrMagnitude;

        if (distanceSqr > config.AttackRange * config.AttackRange)
        {
            MoveTowardsTarget(direction);
            return;
        }

        RotateTowards(direction);
        ApplyMovement(Vector3.zero);

        if (attackCooldownTimer <= 0f)
        {
            AttackCurrentTarget();
        }
    }

    public void Initialize(EnemySquadConfig squadConfig, Health fallbackTargetHealth)
    {
        if (squadConfig == null)
        {
            Debug.LogWarning("EnemySquadController cannot initialize without a config.", this);
            return;
        }

        config = squadConfig;
        fallbackTarget = fallbackTargetHealth;

        if (health == null)
        {
            Debug.LogWarning("EnemySquadController cannot initialize without a Health component.", this);
            IsInitialized = false;
            return;
        }

        health.SetMaxHealth(config.MaxHealth, true);

        if (combatIdentity != null)
        {
            combatIdentity.SetTeam(Team.Enemy);
        }

        SubscribeToHealth();
        isDead = health.IsDead;
        attackCooldownTimer = 0f;
        IsInitialized = true;
    }

    public void SetFallbackTarget(Health target)
    {
        fallbackTarget = target;
    }

    private Health FindTarget()
    {
        if (targetSensor != null && targetSensor.TryFindNearestTarget(out Health detectedTarget))
        {
            return detectedTarget;
        }

        return fallbackTarget != null && !fallbackTarget.IsDead
            ? fallbackTarget
            : null;
    }

    private void MoveTowardsTarget(Vector3 direction)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            ApplyMovement(Vector3.zero);
            return;
        }

        Vector3 moveDirection = direction.normalized;
        RotateTowards(moveDirection);
        ApplyMovement(moveDirection * config.MoveSpeed);
    }

    private void ApplyMovement(Vector3 horizontalVelocity)
    {
        if (characterController == null)
        {
            transform.position += horizontalVelocity * Time.deltaTime;
            LogMissingControllerWarning();
            return;
        }

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        horizontalVelocity.y = verticalVelocity;
        characterController.Move(horizontalVelocity * Time.deltaTime);
    }

    private void RotateTowards(Vector3 direction)
    {
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
        if (currentTarget == null || currentTarget.IsDead)
        {
            currentTarget = null;
            return;
        }

        currentTarget.TakeDamage(CalculateDamage());
        attackCooldownTimer = config.AttackCooldown;
    }

    private float CalculateDamage()
    {
        if (!config.DamageScalesWithMembers)
        {
            return config.Damage;
        }

        int memberCount = Mathf.Max(1, config.MemberCount);
        int aliveMembers = Mathf.Clamp(
            Mathf.CeilToInt(health.HealthNormalized * memberCount),
            1,
            memberCount);
        float multiplier = Mathf.Clamp(
            (float)aliveMembers / memberCount,
            1f / memberCount,
            1f);

        return config.Damage * multiplier;
    }

    private void SubscribeToHealth()
    {
        if (isSubscribedToHealth)
        {
            return;
        }

        health.OnDied += HandleDied;
        isSubscribedToHealth = true;
    }

    private void HandleDied()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentTarget = null;
        OnSquadDied?.Invoke(this);
        Destroy(gameObject, Mathf.Max(0f, destroyDelayAfterDeath));
    }

    private void OnDestroy()
    {
        if (health != null && isSubscribedToHealth)
        {
            health.OnDied -= HandleDied;
            isSubscribedToHealth = false;
        }
    }

    private void LogMissingControllerWarning()
    {
        if (hasLoggedMissingController)
        {
            return;
        }

        hasLoggedMissingController = true;
        Debug.LogWarning(
            "EnemySquadController is missing a CharacterController. Using Transform movement as a fallback.",
            this);
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
}
