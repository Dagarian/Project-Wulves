using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Spawn")]
public class SpawnTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        if(controller.FinishedSpawning())
        {
            return true;
        }
        return false;
    }
}
