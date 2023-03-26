using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Emote")]
public class EmoteTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.FinishedEmoting())
        {
            return true;
        }
        return false;
    }
}
