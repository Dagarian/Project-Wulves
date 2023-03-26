using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/NotInterupt")]
public class NotInteruptTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (controller.GetCombatController().uninterruptible)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
