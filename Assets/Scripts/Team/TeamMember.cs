using UnityEngine;

public class TeamMember : MonoBehaviour
{
    [SerializeField]
    private int teamId = 1; // 1.플레이어, 2.적

    public int GetTeamId()
    {
        return teamId;
    }

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;
    }
}
