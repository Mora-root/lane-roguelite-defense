using UnityEngine;

public sealed class UnitSpawnDebugControls : MonoBehaviour
{
    [SerializeField] private UnitSpawner unitSpawner;
    [SerializeField] private UnitConfig playerUnitConfig;
    [SerializeField] private UnitConfig enemyUnitConfig;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && CanSpawnPlayerUnit())
        {
            unitSpawner.TrySpawnPlayerUnit(playerUnitConfig);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanSpawnEnemyUnit())
        {
            unitSpawner.SpawnEnemyUnit(enemyUnitConfig);
        }
    }

    private bool CanSpawnPlayerUnit()
    {
        if (!HasSpawner())
        {
            return false;
        }

        if (playerUnitConfig == null)
        {
            Debug.LogWarning("UnitSpawnDebugControls cannot spawn a player unit without a player UnitConfig.", this);
            return false;
        }

        return true;
    }

    private bool CanSpawnEnemyUnit()
    {
        if (!HasSpawner())
        {
            return false;
        }

        if (enemyUnitConfig == null)
        {
            Debug.LogWarning("UnitSpawnDebugControls cannot spawn an enemy unit without an enemy UnitConfig.", this);
            return false;
        }

        return true;
    }

    private bool HasSpawner()
    {
        if (unitSpawner == null)
        {
            Debug.LogWarning("UnitSpawnDebugControls cannot spawn units without a UnitSpawner reference.", this);
            return false;
        }

        return true;
    }
}
