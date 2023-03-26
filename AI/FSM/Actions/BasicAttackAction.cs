using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Basic Attack")]
public class BasicAttackAction : AiAction
{
    NpcCombatController combat;
    NpcMovementController movement;
    NpcStats stats;
    Animator animator;

    public override void SetupAction(StateController controller)
    {
        combat = controller.GetCombatController();
        movement = controller.GetMovementController();
        stats = controller.GetStats();
        animator = controller.GetAnimator();
    }
    public override void Act(StateController controller)
    {
        bool attacking = false;
        combat = controller.GetCombatController();
        movement = controller.GetMovementController();
        stats = controller.GetStats();
        movement.StopMoving();

        if (combat.GetTarget() != null)
        {
            bool dead = false;
            NpcStats npcStats = combat.GetTarget().GetComponent<NpcStats>();
            EnvironmentStats envStats = combat.GetTarget().GetComponent<EnvironmentStats>();
            if (npcStats != null)
            {
                dead = npcStats.GetDead();
            }
            else if (envStats != null)
            {
                dead = envStats.GetDead();
            }

            if (!dead)
            {
                //If target not dead, attack! Else reset target.
                stats.SetCombatStatus(true);
                attacking = true;
            }
            else
            {
                combat.SetTarget(null);
            }
        }
        else
        {
            attacking = false;
            stats.SetCombatStatus(false);
            combat.SetTarget(null);
            combat.GetTargetDistance();
        }
        animator.SetBool("Attacking", attacking);
    }
}
