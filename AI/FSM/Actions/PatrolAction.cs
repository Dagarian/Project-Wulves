using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Patrol")]
public class PatrolAction : AiAction
{
    NpcMovementController movement;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        movement.SetAutoBraking(true);
    }
    public override void Act(StateController controller)
    {
        Vector3 destination = movement.GetDestination();
        Transform[] locations = movement.GetDefaultLocations();
        GameObject tempObj = new GameObject();
        tempObj.transform.position = destination;
        movement.ContinueMoving();
        if (!locations.Contains(tempObj.transform))
        {
            destination = movement.GetDefaultLocation();
        }
        movement.SetDestination(destination);
        //Debug.Log("Go to point");
        if (movement.Arrived())
        {
            movement.ChangePatrolPoint();

            //Debug.Log("Change point.");
        }
        GameObject.Destroy(tempObj);
    }
}