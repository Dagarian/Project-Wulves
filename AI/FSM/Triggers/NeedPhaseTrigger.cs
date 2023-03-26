using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/NeedPhase")]
public class NeedPhaseTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (controller.SwapPhase())
        {
            return true;
        }
        return false;
    }
}