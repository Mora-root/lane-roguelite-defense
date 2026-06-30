using System;
using UnityEngine;

public sealed class BaseController : MonoBehaviour
{
    [SerializeField] private Team team = Team.Player;
    [SerializeField] private Health health;

    public event Action<BaseController> OnDestroyed;

    public Team Team => team;
    public Health Health => health;

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandleDied;
        }
    }

    private void HandleDied()
    {
        OnDestroyed?.Invoke(this);
    }
}
