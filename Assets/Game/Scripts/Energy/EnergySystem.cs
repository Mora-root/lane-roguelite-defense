using System;
using UnityEngine;

public sealed class EnergySystem : MonoBehaviour
{
    [SerializeField] private float startingEnergy;
    [SerializeField] private float maxEnergy = 10f;
    [SerializeField] private float generationRate = 1f;
    [SerializeField] private bool autoGenerate = true;

    public event Action<float, float> OnEnergyChanged;
    public event Action<float> OnGenerationRateChanged;

    public float CurrentEnergy { get; private set; }
    public float MaxEnergy => maxEnergy;
    public float GenerationRate => generationRate;
    public bool AutoGenerate => autoGenerate;

    private void Awake()
    {
        maxEnergy = Mathf.Max(1f, maxEnergy);
        generationRate = Mathf.Max(0f, generationRate);
        CurrentEnergy = Mathf.Clamp(startingEnergy, 0f, maxEnergy);

        OnEnergyChanged?.Invoke(CurrentEnergy, maxEnergy);
        OnGenerationRateChanged?.Invoke(generationRate);
    }

    private void Update()
    {
        if (!autoGenerate || CurrentEnergy >= maxEnergy)
        {
            return;
        }

        AddEnergy(generationRate * Time.deltaTime);
    }

    public bool CanSpend(float amount)
    {
        return amount >= 0f && CurrentEnergy >= amount;
    }

    public bool Spend(float amount)
    {
        if (amount < 0f || !CanSpend(amount))
        {
            return false;
        }

        SetCurrentEnergy(CurrentEnergy - amount);
        return true;
    }

    public void AddEnergy(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetCurrentEnergy(CurrentEnergy + amount);
    }

    public void SetMaxEnergy(float value, bool refillEnergy)
    {
        maxEnergy = Mathf.Max(1f, value);
        CurrentEnergy = refillEnergy
            ? maxEnergy
            : Mathf.Clamp(CurrentEnergy, 0f, maxEnergy);

        OnEnergyChanged?.Invoke(CurrentEnergy, maxEnergy);
    }

    public void AddMaxEnergy(float amount, bool refillAddedAmount)
    {
        if (amount <= 0f)
        {
            return;
        }

        maxEnergy = Mathf.Max(1f, maxEnergy + amount);

        if (refillAddedAmount)
        {
            CurrentEnergy += amount;
        }

        CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0f, maxEnergy);
        OnEnergyChanged?.Invoke(CurrentEnergy, maxEnergy);
    }

    public void SetGenerationRate(float value)
    {
        generationRate = Mathf.Max(0f, value);
        OnGenerationRateChanged?.Invoke(generationRate);
    }

    public void AddGenerationRate(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        SetGenerationRate(generationRate + amount);
    }

    public void SetAutoGenerate(bool value)
    {
        autoGenerate = value;
    }

    public void ResetEnergy(float value)
    {
        SetCurrentEnergy(value);
    }

    public void ApplyGeneratorUpgrade(float generationRateIncrease, float maxEnergyIncrease, bool refillAddedCapacity)
    {
        AddGenerationRate(generationRateIncrease);
        AddMaxEnergy(maxEnergyIncrease, refillAddedCapacity);
    }

    private void SetCurrentEnergy(float value)
    {
        CurrentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
        OnEnergyChanged?.Invoke(CurrentEnergy, maxEnergy);
    }
}
