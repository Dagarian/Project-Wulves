using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/False Trigger")]
public class NoTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        return false;
    }
}