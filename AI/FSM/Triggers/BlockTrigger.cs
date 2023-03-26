using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Block")]
public class BlockTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (controller.FinishedBlocking())
        {
            return true;
        }
        return false;
    }
}