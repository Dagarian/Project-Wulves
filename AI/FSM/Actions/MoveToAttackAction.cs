using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Move To Attack")]
public class MoveToAttackAction : AiAction
{
    NpcMovementController movement;
    NpcCombatController combat;
    Animator animator;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        combat = controller.GetCombatController();
        animator = controller.GetAnimator();
        movement.StopMoving();
        animator.SetBool("Attacking", false);
        //animator.SetBool("Blocking", false);
        animator.SetBool("Threatened", false);

        if (combat.GetTarget() != null)
        {
            if (combat.GetTarget().tag != "Player")
            {
                animator.SetFloat("AttackType", 1);
            }
            else
            {
                animator.SetFloat("AttackType", Random.Range(0, combat.attackNumber));
            }
        }

        if (controller.gameObject.name == "Ice Demon")
        {
            combat.SetAggroRange(100);
        }

    }
    public override void Act(StateController controller)
    {
        GameObject target = combat.GetTarget();

        if (target != null) //If target exists
        {
            if (animator)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Telegraph") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attacking") ||
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Post-attack Emote"))
                {
                    movement.StopMoving();
                }
                else
                {
                    movement.FaceTarget();
                    movement.ContinueMoving();
                    movement.SetDestination(target.transform.position);
                }

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Post-attack Emote"))
                {
                    movement.FaceTarget();
                }
            }
        }
    }
}
