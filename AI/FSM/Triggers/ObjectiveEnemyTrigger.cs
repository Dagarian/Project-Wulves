using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/ObjectiveEnemy")]
public class ObjectiveEnemyTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.ObjectiveEnemy())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
