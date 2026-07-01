using UnityEngine;

[CreateAssetMenu(fileName = "EnemySquadConfig", menuName = "Game/Enemies/Enemy Squad Config")]
public sealed class EnemySquadConfig : ScriptableObject
{
    [SerializeField] private string squadId;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int memberCount = 5;
    [SerializeField] private bool damageScalesWithMembers;

    public string SquadId => squadId;
    public string DisplayName => displayName;
    public GameObject Prefab => prefab;
    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float Damage => damage;
    public float AttackCooldown => attackCooldown;
    public int MemberCount => memberCount;
    public bool DamageScalesWithMembers => damageScalesWithMembers;

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        moveSpeed = Mathf.Max(0f, moveSpeed);
        attackRange = Mathf.Max(0.1f, attackRange);
        damage = Mathf.Max(0f, damage);
        attackCooldown = Mathf.Max(0.05f, attackCooldown);
        memberCount = Mathf.Max(1, memberCount);
    }
}
