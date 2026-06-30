using System;
using UnityEngine;

public sealed class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private bool hasDied;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDied;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead => hasDied;
    public float HealthNormalized => maxHealth > 0f
        ? Mathf.Clamp01(CurrentHealth / maxHealth)
        : 0f;

    private void Awake()
    {
        maxHealth = Mathf.Max(1f, maxHealth);
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount < 0f || hasDied)
        {
            return;
        }

        SetCurrentHealth(CurrentHealth - amount);
    }

    public void Heal(float amount)
    {
        if (amount < 0f || hasDied)
        {
            return;
        }

        SetCurrentHealth(CurrentHealth + amount);
    }

    public void SetMaxHealth(float value, bool refillHealth)
    {
        float previousMaxHealth = maxHealth;
        maxHealth = Mathf.Max(1f, value);

        float newHealth = refillHealth && !hasDied
            ? maxHealth
            : Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        bool maxHealthChanged = !Mathf.Approximately(previousMaxHealth, maxHealth);
        SetCurrentHealth(newHealth, maxHealthChanged);
    }

    public void Kill()
    {
        if (hasDied)
        {
            return;
        }

        SetCurrentHealth(0f);
    }

    private void SetCurrentHealth(float value, bool forceHealthChangedEvent = false)
    {
        float clampedHealth = Mathf.Clamp(value, 0f, maxHealth);
        bool healthChanged = !Mathf.Approximately(CurrentHealth, clampedHealth);

        CurrentHealth = clampedHealth;

        if (healthChanged || forceHealthChangedEvent)
        {
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        if (CurrentHealth <= 0f && !hasDied)
        {
            hasDied = true;
            OnDied?.Invoke();
        }
    }
}
