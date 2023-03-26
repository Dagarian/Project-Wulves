using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Camp Location")]
public class CampAction : AiAction
{
    NpcMovementController movement;
    Animator animator;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetAnimator();

        animator.SetBool("Threatened", false);
        animator.SetBool("Attacking", false);
        
        NpcStats stats = controller.GetStats();

        if (movement.GetSpawn() != null)
        {
            Vector3 destination = movement.GetSpawn().transform.position;
            movement.SetDestination(destination);
        }
    }
    public override void Act(StateController controller)
    {
        //if (animator != null)
        //{
        //    animator.SetBool("Attacking", false);
        //}
        if (movement != null)
        {
            if (movement.Arrived())
            {
                
                movement.StopMoving();
            }
        }
    }
}
