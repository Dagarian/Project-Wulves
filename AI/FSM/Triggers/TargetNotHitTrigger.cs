using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/TargetNotHit")]
public class TargetNotHitTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (!controller.GetCombatController().TargetHit())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}