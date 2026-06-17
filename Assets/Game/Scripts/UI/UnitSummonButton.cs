using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UnitSummonButton : MonoBehaviour
{
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private UnitConfig unitConfig;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private string labelFormat = "{0}\nCost: {1:0}";

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
        else
        {
            Debug.LogWarning("UnitSummonButton is missing a Button reference.", this);
        }

        if (energySystem != null)
        {
            energySystem.OnEnergyChanged += HandleEnergyChanged;
        }
        else
        {
            Debug.LogWarning("UnitSummonButton is missing an EnergySystem reference.", this);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }

        if (energySystem != null)
        {
            energySystem.OnEnergyChanged -= HandleEnergyChanged;
        }
    }

    private void HandleClick()
    {
        if (HasRequiredReferences())
        {
            unitSpawner.TrySpawnPlayerUnit(unitConfig);
        }

        Refresh();
    }

    private void HandleEnergyChanged(float current, float max)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (labelText != null && unitConfig != null)
        {
            labelText.text = string.Format(labelFormat, unitConfig.DisplayName, unitConfig.EnergyCost);
        }
        else if (labelText == null)
        {
            Debug.LogWarning("UnitSummonButton is missing a TMP_Text label reference.", this);
        }

        if (button != null)
        {
            button.interactable = CanSummon();
        }
    }

    private bool CanSummon()
    {
        return unitSpawner != null
            && energySystem != null
            && unitConfig != null
            && energySystem.CanSpend(unitConfig.EnergyCost);
    }

    private bool HasRequiredReferences()
    {
        bool hasReferences = true;

        if (unitSpawner == null)
        {
            Debug.LogWarning("UnitSummonButton cannot summon without a UnitSpawner reference.", this);
            hasReferences = false;
        }

        if (energySystem == null)
        {
            Debug.LogWarning("UnitSummonButton cannot summon without an EnergySystem reference.", this);
            hasReferences = false;
        }

        if (unitConfig == null)
        {
            Debug.LogWarning("UnitSummonButton cannot summon without a UnitConfig.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
