using UnityEngine;

[CreateAssetMenu(menuName = "Game/Units/Unit Config")]
public sealed class UnitConfig : ScriptableObject
{
    [SerializeField] private string unitId;
    [SerializeField] private string displayName;
    [SerializeField] private Team team;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float energyCost = 2f;
    [SerializeField] private bool isRanged;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 8f;

    public string UnitId => unitId;
    public string DisplayName => displayName;
    public Team Team => team;
    public GameObject Prefab => prefab;
    public float MaxHealth => maxHealth;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float EnergyCost => energyCost;
    public bool IsRanged => isRanged;
    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        damage = Mathf.Max(0f, damage);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        attackRange = Mathf.Max(0f, attackRange);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
        energyCost = Mathf.Max(0f, energyCost);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);
    }
}
