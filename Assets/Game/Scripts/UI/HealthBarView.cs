using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HealthBarView : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private string valueFormat = "{0:0}/{1:0}";
    [SerializeField] private bool showValueText;

    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        ConfigureSlider();
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += HandleHealthChanged;
        }
        else
        {
            Debug.LogWarning("HealthBarView is missing a Health reference.", this);
        }

        if (slider == null)
        {
            Debug.LogWarning("HealthBarView is missing a Slider reference.", this);
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(float current, float max)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (targetHealth == null || slider == null)
        {
            return;
        }

        float maxHealth = targetHealth.MaxHealth;
        float normalized = maxHealth > 0f ? targetHealth.CurrentHealth / maxHealth : 0f;
        slider.value = Mathf.Clamp01(normalized);

        if (valueText == null)
        {
            return;
        }

        valueText.text = showValueText
            ? string.Format(valueFormat, targetHealth.CurrentHealth, maxHealth)
            : string.Empty;
    }

    private void ConfigureSlider()
    {
        if (slider == null)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.interactable = false;
    }
}
