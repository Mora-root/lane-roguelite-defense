using TMPro;
using UnityEngine;

public sealed class EnergyDebugView : MonoBehaviour
{
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private TMP_Text energyText;

    private void OnEnable()
    {
        if (!HasRequiredReferences())
        {
            return;
        }

        energySystem.OnEnergyChanged += HandleEnergyChanged;
        HandleEnergyChanged(energySystem.CurrentEnergy, energySystem.MaxEnergy);
    }

    private void OnDisable()
    {
        if (energySystem != null)
        {
            energySystem.OnEnergyChanged -= HandleEnergyChanged;
        }
    }

    private void HandleEnergyChanged(float current, float max)
    {
        if (energyText == null)
        {
            return;
        }

        energyText.text = $"Energy: {current:0.0} / {max:0.0}";
    }

    private bool HasRequiredReferences()
    {
        if (energySystem == null)
        {
            Debug.LogWarning($"{nameof(EnergyDebugView)} on '{name}' is missing an EnergySystem reference.", this);
            return false;
        }

        if (energyText == null)
        {
            Debug.LogWarning($"{nameof(EnergyDebugView)} on '{name}' is missing a TMP_Text reference.", this);
            return false;
        }

        return true;
    }
}
