using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Manual Trigger")]
public class ManualTrigger : StateTrigger
{
    bool toTrigger;
    public override bool TriggerState(StateController controller)
    {
        if (controller.GetManualTrigger())
        {
            return true;
        }
        return false;
    }
}
