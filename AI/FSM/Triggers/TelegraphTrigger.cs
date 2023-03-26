using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Telegraph")]
public class TelegraphTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.FinishedTelegraph())
        {
            return true;
        }
        return false;
    }
}
