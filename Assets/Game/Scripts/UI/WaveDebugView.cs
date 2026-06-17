using TMPro;
using UnityEngine;

public sealed class WaveDebugView : MonoBehaviour
{
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TMP_Text waveText;

    private void Awake()
    {
        if (waveText == null)
        {
            waveText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (waveManager == null)
        {
            Debug.LogWarning("WaveDebugView is missing a WaveManager reference.", this);
        }
        else
        {
            waveManager.OnWaveStarted += HandleWaveChanged;
            waveManager.OnWaveCleared += HandleWaveChanged;
            waveManager.OnWaveFailed += HandleWaveChanged;
            waveManager.OnAliveEnemiesChanged += HandleAliveEnemiesChanged;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (waveManager == null)
        {
            return;
        }

        waveManager.OnWaveStarted -= HandleWaveChanged;
        waveManager.OnWaveCleared -= HandleWaveChanged;
        waveManager.OnWaveFailed -= HandleWaveChanged;
        waveManager.OnAliveEnemiesChanged -= HandleAliveEnemiesChanged;
    }

    private void HandleWaveChanged()
    {
        Refresh();
    }

    private void HandleAliveEnemiesChanged(int aliveEnemies)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (waveText == null)
        {
            Debug.LogWarning("WaveDebugView is missing a TMP_Text reference.", this);
            return;
        }

        if (waveManager == null)
        {
            Debug.LogWarning("WaveDebugView cannot refresh without a WaveManager reference.", this);
            return;
        }

        waveText.text =
            $"Wave: {GetWaveStatus()}\n" +
            $"Enemies Alive: {waveManager.AliveEnemies}\n" +
            $"Enemies Spawned: {waveManager.SpawnedEnemies}";
    }

    private string GetWaveStatus()
    {
        if (waveManager.IsWaveFailed)
        {
            return "Failed";
        }

        if (waveManager.IsWaveComplete)
        {
            return "Cleared";
        }

        return waveManager.IsWaveRunning ? "Running" : "Not Running";
    }
}
