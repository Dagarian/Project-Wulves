using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Dead")]
public class DeadTrigger : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        NpcStats stats = controller.GetStats();
        if (stats.GetDead())
        {
            //Debug.Log("Dead");
            return true;
        }
        return false;
    }
}