using Mirror;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class <c>UnitMovement</c> is a Mirror component script used to manage the player unit movement with the networking aspects of the game.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : NetworkBehaviour
{
    /// <summary>
    /// Instance variable <c>agent</c> is a Unity <c>NavMeshAgent</c> component representing the navigation meshing agent of the player character.
    /// </summary>
    [SerializeField] private NavMeshAgent agent;

    /// <summary>
    /// Instance variable <c>targeter</c> is a Mirror <c>Targeter</c> component script representing the targeter behaviour manager of the player unit.
    /// </summary>
    [SerializeField] private Targeter targeter;

    /// <summary>
    /// Instance variable <c>chaseRange</c> represents the maximum range value a unit will follow a target.
    /// </summary>
    [SerializeField] private float chaseRange = 10f;

    /// <summary>
    /// Instance variable <c>animator</c> is a Unity <c>Animator</c> component representing the unit animator.
    /// </summary>
    [SerializeField] private Animator animator;
    
    #region Server

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become active on the server.
    /// </summary>
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    /// <summary>
    /// This function is called for NetworkBehaviour objects when they become inactive on the server.
    /// </summary>
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    [ServerCallback]
    private void Update()
    {
        if (targeter.GetTarget())
        {
            if ((targeter.GetTarget().transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(targeter.GetTarget().transform.position);
            } 
            else if (agent.hasPath)
            {   
                agent.ResetPath();
            }
            
            return;
        }

        if (agent.hasPath)
        {
            return;
        }
        
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            return;
        }
        
        agent.ResetPath();
    }

    /// <summary>
    /// This function is used by the server on a client to check for a valid position and then to move it the client player character.
    /// </summary>
    /// <param name="position">A Unity <c>Vector3</c> component representing the position to move on.</param>
    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }
    
    /// <summary>
    /// This server-side function is responsible for moving units on the terrain.
    /// </summary>
    /// <param name="position">A Unity <c>Vector</c> component representing the position to move the unit on.</param>
    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();
        
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }

    /// <summary>
    /// This server side function is responsible for resetting all agent paths on game over event trigger.
    /// </summary>
    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion
}
