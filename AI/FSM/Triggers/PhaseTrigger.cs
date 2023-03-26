using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Phase")]
public class PhaseTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (controller.FinishedPhasing() && controller.FinishedAttacking())
        {
            return true;
        }
        return false;
    }
}