using Mirror;
using UnityEngine;

public class InfantryUnitMovement : UnitMovement
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [Server]
    public override void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        agent.speed = Vector3.Distance(transform.position, position) > 20 ? runSpeed : walkSpeed;

        agent.SetDestination(position);
    }
}
