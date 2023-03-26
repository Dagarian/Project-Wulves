using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Move To Point")]
public class MoveToPointAction : AiAction
{
    NpcMovementController movement;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
    }
    public override void Act(StateController controller)
    {
        movement.ContinueMoving();
        movement.SetDestinationToDefault();
    }
}
