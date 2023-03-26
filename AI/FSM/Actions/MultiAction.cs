using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Multi")]
public class MultiAction : AiAction
{
    NpcMovementController movement;
    NpcCombatController combat;
    Animator animator;
    int action;
    State state;
    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponent<Animator>();
        combat = controller.GetCombatController();

        action = Random.Range(0, 8);

        movement.StopMoving();
        movement.FaceTarget();

        if (controller.multiState.Length > 1)
        {
            if (combat.GetTimeSinceAttack() > 5)
            {
                state = controller.GetMultiState(0);
            }
            else
            {
                if (action < 3)
                {
                    //Debug.Log("ATTACK");
                    state = controller.GetMultiState(0);
                }
                if (action >= 3 && action < 6)
                {
                    //Debug.Log("BLOCK");
                    state = controller.GetMultiState(1);
                    animator.SetBool("Blocking", true);
                }
                if (action >= 6 && action < 9)
                {

                    //Debug.Log("DODGE");
                    state = controller.GetMultiState(2);
                }
            }
        }
        else
        {
            state = controller.GetMultiState(0);
        }
        controller.TransitionToState(state);

    }
    public override void Act(StateController controller)
    {
        //movement.StopMoving();
        //movement.FaceTarget();

        //State state = (State)ScriptableObject.CreateInstance("State");
        //state.actions = new AiAction[1];


        //if (action < 3)
        //{
        //    state.name = "Move To Attack";
        //    state.actions[0] = (MoveToAttackAction)ScriptableObject.CreateInstance("MoveToAttackAction");
        //    controller.TransitionToState(state);
        //}
        //else if (action >= 3 || action < 6)
        //{

        //}
        //else if (action >= 6 || action < 9)
        //{

        //}
    }
}