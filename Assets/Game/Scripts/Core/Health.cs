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

    private void Awake()
    {
        maxHealth = Mathf.Max(0f, maxHealth);
        CurrentHealth = maxHealth;
        hasDied = CurrentHealth <= 0f;
    }

    public void TakeDamage(float amount)
    {
        if (hasDied || amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void SetMaxHealth(float value, bool refillHealth)
    {
        maxHealth = Mathf.Max(0f, value);
        CurrentHealth = refillHealth
            ? maxHealth
            : Mathf.Clamp(CurrentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (!hasDied && CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (hasDied)
        {
            return;
        }

        hasDied = true;
        OnDied?.Invoke();
    }
}
