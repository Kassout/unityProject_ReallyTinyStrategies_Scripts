using Mirror;
using UnityEngine;

public class MachineUnitMovement : UnitMovement
{
    [SerializeField]
    private float movementSpeed;

    private void Start()
    {
        agent.speed = movementSpeed;
    }

    [Server]
    public override void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        agent.SetDestination(position);
    }
}
