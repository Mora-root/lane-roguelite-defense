using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Waves/Wave Config")]
public sealed class WaveConfig : ScriptableObject
{
    [SerializeField] private string waveId;
    [SerializeField] private string displayName;
    [SerializeField] private List<WaveSpawnEntry> spawnEntries = new List<WaveSpawnEntry>();

    public string WaveId => waveId;
    public string DisplayName => displayName;
    public IReadOnlyList<WaveSpawnEntry> SpawnEntries => spawnEntries;

    private void OnValidate()
    {
        if (spawnEntries == null)
        {
            return;
        }

        for (int i = 0; i < spawnEntries.Count; i++)
        {
            if (spawnEntries[i] != null)
            {
                spawnEntries[i].ClampValues();
            }
        }
    }
}

[Serializable]
public sealed class WaveSpawnEntry
{
    [SerializeField] private UnitConfig unitConfig;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int count = 1;
    [SerializeField] private float interval;

    public UnitConfig UnitConfig => unitConfig;
    public float SpawnDelay => spawnDelay;
    public int Count => count;
    public float Interval => interval;

    public void ClampValues()
    {
        spawnDelay = Mathf.Max(0f, spawnDelay);
        count = Mathf.Max(1, count);
        interval = Mathf.Max(0f, interval);
    }
}
