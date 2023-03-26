using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Block")]
public class BlockAction : AiAction
{
    NpcMovementController movement;
    Animator animator;

    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponent<Animator>();
        animator.SetBool("Blocking", true);
    }
    public override void Act(StateController controller)
    {
        movement.StopMoving();
        movement.FaceTarget();
    }
}