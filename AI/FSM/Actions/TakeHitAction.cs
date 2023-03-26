using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/TakeHit")]
public class TakeHitAction : AiAction
{
    NpcMovementController movement;
    Animator animator;
    float timer;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponentInChildren<Animator>();
    }
    public override void Act(StateController controller)
    {
        movement.StopMoving();
        movement.FaceTarget();
        if (animator.GetBool("Damaged"))
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                animator.SetBool("Damaged", false);
                timer = 0;
            }
        }
    }
}