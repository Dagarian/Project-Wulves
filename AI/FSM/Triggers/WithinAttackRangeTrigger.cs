using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Target In Attack Range")]
public class WithinAttackRangeTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        
        NpcCombatController combat = controller.GetCombatController();
        float targetDistance = combat.GetTargetDistance();
        float attackRange = combat.GetAttackRange();
        if (combat.WithinAttackRange())
        {
            Debug.Log("Player Within Attack Range");
            return true;
        }
        return false;
    }
}
