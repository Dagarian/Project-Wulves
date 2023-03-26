using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Death")]
public class DeathAction : AiAction
{
    bool trackKill = true;
    Animator animator;
    public override void SetupAction(StateController controller)
    {
        animator = controller.gameObject.GetComponentInChildren<Animator>();
        controller.GetComponent<NpcStats>().Harm(1000);
    }
    public override void Act(StateController controller)
    {
        animator = controller.gameObject.GetComponentInChildren<Animator>();
        controller.GetMovementController().StopMoving();
        controller.GetMovementController().currentTarget = null;

        animator.SetBool("Attacking", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Threatened", false);
        animator.SetBool("Death", true);
        //Debug.Log("Set Animations on death");
        if (trackKill)
        {
            Mission missionManager = GameObject.Find("MissionManager")?.GetComponent<Mission>();
            
            if (missionManager == null)
            {
                // Warning: the tutorial spawns an enemy, but doesnt have a missionManager active, so this is a Warning rather than an Error.
                Debug.LogWarning("The Mission Manager was not found.");
                return;
            }

            switch (controller.GetComponent<Transform>().name)
            {
                case "Ice Demon":
                    missionManager.totalT1Eliminated++;
                    break;

                case "Skeleton Soldier":
                    missionManager.totalT2Eliminated++;
                    break;

                case "Ice Golem":
                    missionManager.totalT3Eliminated++;
                    break;

                default:
                    break;
            }
            trackKill = false;
        }
        //Debug.Log(controller.gameObject.name + " has died. Waiting for despawn.");
    }
}
