using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Dodge")]
public class DodgeTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if (controller.FinishedDodging())
        {
            return true;
        }
        return false;
    }
}