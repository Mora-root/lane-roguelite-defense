using UnityEngine;

public sealed class CombatIdentity : MonoBehaviour
{
    [SerializeField] private Team team = Team.Player;
    [SerializeField] private bool isTargetable = true;

    public Team Team => team;
    public bool IsTargetable => isTargetable;

    public void SetTeam(Team value)
    {
        team = value;
    }

    public void SetTargetable(bool value)
    {
        isTargetable = value;
    }
}
