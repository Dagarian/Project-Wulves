using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Attack")]
public class AttackTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.FinishedAttacking())
        {
            return true;
        }
        return false;
    }
}
