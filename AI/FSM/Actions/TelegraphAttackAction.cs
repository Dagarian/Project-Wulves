using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/TelegraphAttack")]
public class TelegraphAttackAction : AiAction
{
    NpcMovementController movement;
    NpcCombatController combat;
    Animator animator;
    bool attacking;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponent<Animator>();
        combat = controller.GetCombatController();
        attacking = false;

        //animator.SetFloat("AttackType", 0);
        movement.FaceTarget();
        //animator.SetFloat("AttackType", Random.Range(0, 1));
        //Debug.Log(animator.GetFloat("AttackType"));
    }
    public override void Act(StateController controller)
    {
        GameObject target = combat.GetTarget();

        movement.StopMoving();

        if (!attacking)
        {
            movement.FaceTarget();
        }
        if (combat.GetTarget())
        {
            float angle = Vector3.Angle(controller.gameObject.transform.position, combat.GetTarget().transform.position);
            if (target != null && (angle <= 15 || angle >= -15))
            {
                attacking = true;
            }
        }
        animator.SetBool("Attacking", attacking);
    }
}
