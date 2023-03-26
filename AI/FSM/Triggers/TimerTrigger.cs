using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Timer")]
public class TimerTrigger : StateTrigger
{
    public float time;
    public override bool TriggerState(StateController controller)
    {
        if(controller.CheckIfCountdownElapsed(time))
        {
            return true;
        }
        return false;
    }
}
