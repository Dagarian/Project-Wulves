using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Dodge")]
public class DodgeAction : AiAction
{
    NpcMovementController movement;
    Animator animator;
    float dodgeLength;
    float timer;
    bool dodging;

    public override void SetupAction(StateController controller)
    {
        movement = controller.GetMovementController();
        animator = controller.GetComponent<Animator>();
        animator.SetBool("Dodge", true);
        timer = 0;
        dodging = true;
    }
    public override void Act(StateController controller)
    {

        movement.StopMoving();
        movement.FaceTarget();

        timer += Time.deltaTime;
        if (timer > 1)
        {
            dodging = false;
        }

        animator.SetBool("Dodge", dodging);

    }
}