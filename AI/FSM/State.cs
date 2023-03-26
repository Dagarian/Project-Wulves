using UnityEngine;

[CreateAssetMenu(menuName = "AI/State")]
public class State : ScriptableObject
{

    public AiAction[] actions;
    public Transition[] transitions;
    bool setupComplete;

    public void UpdateState(StateController controller)
    {
        if (setupComplete)
        {
            Act(controller);
            Check(controller);
        }
    }
    
    public void SetupAction(StateController controller)
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].SetupAction(controller);
            //Debug.Log("State-side Setup: " + actions[i].name);
        }
        setupComplete = true;
    }

    private void Act(StateController controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }

    private void Check(StateController controller)
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                StateTrigger[] triggers = transitions[i].triggers;
                bool allTriggersNeeded = transitions[i].AllTriggersNeeded;
                bool transition = false;

                //If all triggers needed (AND)
                if (allTriggersNeeded)
                {
                    //Debug.Log("All triggers needed");
                    transition = true; //Assume all true, check if incorrect.
                    for (int j = 0; j < triggers.Length; j++)
                    {
                        bool trigger = triggers[j].TriggerState(controller);
                        if (!trigger) //If any trigger false, transition false.
                        {
                            transition = false;
                        }
                    }
                }
                else //else, any transition. (OR)
                {
                    //Debug.Log("Any trigger needed");
                    transition = false; //Assume none
                    for (int j = 0; j < triggers.Length; j++)
                    {
                        bool trigger = triggers[j].TriggerState(controller);
                        if (trigger) //If any trigger true, transition true.
                        {
                            transition = true;
                        }
                    }
                }

                if (transition)
                {
                    //Debug.Log("Transition to true state.");
                    controller.TransitionToState(transitions[i].trueState);
                }
                else
                {
                    //Debug.Log("Transition to false state.");
                    controller.TransitionToState(transitions[i].falseState);
                }
            }
        }
    }
}