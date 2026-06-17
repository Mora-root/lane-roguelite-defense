using UnityEngine;

public sealed class UnitSpawner : MonoBehaviour
{
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private Transform unitsRoot;

    public bool TrySpawnPlayerUnit(UnitConfig unitConfig)
    {
        if (!IsValidUnitConfig(unitConfig))
        {
            return false;
        }

        if (energySystem == null)
        {
            Debug.LogWarning("UnitSpawner cannot spawn a player unit without an EnergySystem reference.", this);
            return false;
        }

        if (!energySystem.CanSpend(unitConfig.EnergyCost))
        {
            return false;
        }

        return SpawnUnit(unitConfig, playerSpawnPoint, true) != null;
    }

    public UnitController SpawnEnemyUnit(UnitConfig unitConfig)
    {
        if (!IsValidUnitConfig(unitConfig))
        {
            return null;
        }

        return SpawnUnit(unitConfig, enemySpawnPoint, false);
    }

    public UnitController SpawnUnit(UnitConfig unitConfig, Transform spawnPoint, bool spendEnergy)
    {
        if (!IsValidUnitConfig(unitConfig))
        {
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("UnitSpawner cannot spawn a unit without a spawn point.", this);
            return null;
        }

        if (spendEnergy && !SpendEnergyFor(unitConfig))
        {
            return null;
        }

        GameObject spawnedObject = Instantiate(unitConfig.Prefab, spawnPoint.position, spawnPoint.rotation);
        if (unitsRoot != null)
        {
            spawnedObject.transform.SetParent(unitsRoot, true);
        }

        UnitController unitController = spawnedObject.GetComponent<UnitController>();
        if (unitController == null)
        {
            Debug.LogError("Spawned unit prefab is missing a UnitController component.", spawnedObject);
            Destroy(spawnedObject);
            return null;
        }

        unitController.Initialize(unitConfig);
        return unitController;
    }

    private bool SpendEnergyFor(UnitConfig unitConfig)
    {
        if (energySystem == null)
        {
            Debug.LogWarning("UnitSpawner cannot spend energy without an EnergySystem reference.", this);
            return false;
        }

        return energySystem.Spend(unitConfig.EnergyCost);
    }

    private bool IsValidUnitConfig(UnitConfig unitConfig)
    {
        if (unitConfig == null)
        {
            Debug.LogWarning("UnitSpawner cannot spawn a unit without a UnitConfig.", this);
            return false;
        }

        if (unitConfig.Prefab == null)
        {
            Debug.LogWarning("UnitSpawner cannot spawn a unit because UnitConfig prefab is missing.", this);
            return false;
        }

        return true;
    }
}
