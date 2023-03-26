using UnityEngine;

public abstract class StateTrigger : ScriptableObject
{
    public abstract bool TriggerState(StateController controller);
}
