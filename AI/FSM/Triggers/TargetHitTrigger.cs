using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/TargetHit")]
public class TargetHitTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.GetCombatController().TargetHit())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
