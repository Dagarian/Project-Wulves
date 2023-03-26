using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Within Leash Range")]
public class WithinLeashTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        NpcMovementController movement = controller.GetComponent<NpcMovementController>();
        if(movement.WithinLeashRange())
        {
            return true;
        }
        movement.SetDestinationToDefault();
        return false;
    }
}
