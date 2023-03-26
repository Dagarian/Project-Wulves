using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Target In Aggro Range")]
public class TargetInAggroRangeTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        NpcCombatController combat = controller.GetCombatController();
        combat.SetTarget(combat.FindClosestTargetWithTag());
        GameObject target = combat.GetTarget();
        float targetDistance = combat.GetTargetDistance();
        if(target != null && targetDistance < combat.GetAggroRange())
        {
            //Debug.Log("Target in range");
            return true;
        }
        combat.SetTarget(null);
        //Debug.Log("No Target in range");
        return false;
    }
}
