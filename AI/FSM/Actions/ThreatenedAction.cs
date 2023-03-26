using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Threatened")]
public class ThreatenedAction : AiAction
{
    NpcMovementController movement;
    Animator animator;
    float timer;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponentInChildren<Animator>();
        animator.SetBool("Threatened", true);
        movement.StopMoving();
        movement.FaceTarget();
        
        timer = 0;
    }
    public override void Act(StateController controller)
    {
        timer += Time.deltaTime;
        movement.FaceTarget();
        movement.StopMoving();

        if (timer > animator.GetCurrentAnimatorClipInfo(0).Length)
        {
            animator.SetBool("Threatened", false);
        }
        //else
        //{
        //    animator.SetBool("Threatened", true);

        //}

    }
}