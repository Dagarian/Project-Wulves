[System.Serializable]
public class Transition
{
    public StateTrigger[] triggers;
    public State trueState;
    public State falseState;
    public bool AllTriggersNeeded;
}