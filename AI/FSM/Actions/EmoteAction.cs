using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Emote")]
public class EmoteAction : AiAction
{
    NpcMovementController movement;
    Animator animator;
    float timer;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponent<Animator>();
    }
    public override void Act(StateController controller)
    {
        movement.StopMoving();
        movement.FaceTarget();
        animator.SetBool("Attacking", false);
    }
}