using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private BaseController playerBase;
    [SerializeField] private bool startOnPlay = true;

    private readonly List<UnitController> trackedEnemies = new List<UnitController>();
    private Coroutine spawningCoroutine;
    private bool isSpawningFinished;
    private bool isSubscribedToPlayerBase;

    public event Action OnWaveStarted;
    public event Action OnWaveCleared;
    public event Action OnWaveFailed;
    public event Action<int> OnAliveEnemiesChanged;

    public bool IsWaveRunning { get; private set; }
    public bool IsWaveComplete { get; private set; }
    public bool IsWaveFailed { get; private set; }
    public int AliveEnemies { get; private set; }
    public int SpawnedEnemies { get; private set; }

    private void Start()
    {
        if (startOnPlay)
        {
            StartWave();
        }
    }

    private void OnDestroy()
    {
        StopWave();
    }

    public void StartWave()
    {
        if (IsWaveRunning)
        {
            return;
        }

        if (!HasRequiredReferences())
        {
            return;
        }

        ResetRuntimeState();
        IsWaveRunning = true;

        SubscribeToPlayerBase();
        spawningCoroutine = StartCoroutine(SpawnWave());
        OnWaveStarted?.Invoke();
    }

    public void StopWave()
    {
        StopSpawningCoroutine();
        UnsubscribeFromPlayerBase();
        UnsubscribeFromTrackedEnemies();
        IsWaveRunning = false;
        isSpawningFinished = false;
    }

    private IEnumerator SpawnWave()
    {
        IReadOnlyList<WaveSpawnEntry> entries = waveConfig.SpawnEntries;
        for (int entryIndex = 0; entryIndex < entries.Count; entryIndex++)
        {
            WaveSpawnEntry entry = entries[entryIndex];
            if (entry == null)
            {
                continue;
            }

            if (entry.SpawnDelay > 0f)
            {
                yield return new WaitForSeconds(entry.SpawnDelay);
            }

            for (int spawnIndex = 0; spawnIndex < entry.Count; spawnIndex++)
            {
                if (!IsWaveRunning)
                {
                    yield break;
                }

                SpawnEnemy(entry.UnitConfig);

                if (spawnIndex < entry.Count - 1 && entry.Interval > 0f)
                {
                    yield return new WaitForSeconds(entry.Interval);
                }
            }
        }

        isSpawningFinished = true;
        spawningCoroutine = null;
        TryClearWave();
    }

    private void SpawnEnemy(UnitConfig unitConfig)
    {
        UnitController enemy = unitSpawner.SpawnEnemyUnit(unitConfig);
        if (enemy == null)
        {
            return;
        }

        SpawnedEnemies++;
        AliveEnemies++;
        trackedEnemies.Add(enemy);
        enemy.OnUnitDied += HandleEnemyDied;
        OnAliveEnemiesChanged?.Invoke(AliveEnemies);
    }

    private void HandleEnemyDied(UnitController enemy)
    {
        if (enemy != null)
        {
            enemy.OnUnitDied -= HandleEnemyDied;
            trackedEnemies.Remove(enemy);
        }

        AliveEnemies = Mathf.Max(0, AliveEnemies - 1);
        OnAliveEnemiesChanged?.Invoke(AliveEnemies);
        TryClearWave();
    }

    private void HandlePlayerBaseDestroyed(BaseController destroyedBase)
    {
        FailWave();
    }

    private void TryClearWave()
    {
        if (!IsWaveRunning || IsWaveComplete || IsWaveFailed)
        {
            return;
        }

        if (isSpawningFinished && AliveEnemies == 0 && IsPlayerBaseAlive())
        {
            ClearWave();
        }
    }

    private void ClearWave()
    {
        if (IsWaveComplete || IsWaveFailed)
        {
            return;
        }

        IsWaveComplete = true;
        IsWaveRunning = false;
        UnsubscribeFromPlayerBase();
        UnsubscribeFromTrackedEnemies();
        OnWaveCleared?.Invoke();
    }

    private void FailWave()
    {
        if (IsWaveFailed || IsWaveComplete)
        {
            return;
        }

        IsWaveFailed = true;
        IsWaveRunning = false;
        StopSpawningCoroutine();
        UnsubscribeFromPlayerBase();
        UnsubscribeFromTrackedEnemies();
        OnWaveFailed?.Invoke();
    }

    private void ResetRuntimeState()
    {
        StopSpawningCoroutine();
        UnsubscribeFromPlayerBase();
        UnsubscribeFromTrackedEnemies();

        IsWaveRunning = false;
        IsWaveComplete = false;
        IsWaveFailed = false;
        AliveEnemies = 0;
        SpawnedEnemies = 0;
        isSpawningFinished = false;
    }

    private bool HasRequiredReferences()
    {
        if (waveConfig == null)
        {
            Debug.LogWarning("WaveManager cannot start without a WaveConfig.", this);
            return false;
        }

        if (unitSpawner == null)
        {
            Debug.LogWarning("WaveManager cannot start without a UnitSpawner reference.", this);
            return false;
        }

        if (playerBase == null)
        {
            Debug.LogWarning("WaveManager cannot start without a PlayerBase reference.", this);
            return false;
        }

        if (playerBase.Health == null)
        {
            Debug.LogWarning("WaveManager cannot start because PlayerBase is missing a Health reference.", this);
            return false;
        }

        if (playerBase.Health.IsDead)
        {
            Debug.LogWarning("WaveManager cannot start because PlayerBase is already destroyed.", this);
            return false;
        }

        return true;
    }

    private bool IsPlayerBaseAlive()
    {
        return playerBase != null && playerBase.Health != null && !playerBase.Health.IsDead;
    }

    private void SubscribeToPlayerBase()
    {
        if (playerBase == null || isSubscribedToPlayerBase)
        {
            return;
        }

        playerBase.OnDestroyed += HandlePlayerBaseDestroyed;
        isSubscribedToPlayerBase = true;
    }

    private void UnsubscribeFromPlayerBase()
    {
        if (playerBase == null || !isSubscribedToPlayerBase)
        {
            return;
        }

        playerBase.OnDestroyed -= HandlePlayerBaseDestroyed;
        isSubscribedToPlayerBase = false;
    }

    private void UnsubscribeFromTrackedEnemies()
    {
        for (int i = 0; i < trackedEnemies.Count; i++)
        {
            if (trackedEnemies[i] != null)
            {
                trackedEnemies[i].OnUnitDied -= HandleEnemyDied;
            }
        }

        trackedEnemies.Clear();
    }

    private void StopSpawningCoroutine()
    {
        if (spawningCoroutine == null)
        {
            return;
        }

        StopCoroutine(spawningCoroutine);
        spawningCoroutine = null;
    }
}
