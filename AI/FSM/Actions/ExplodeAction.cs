
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Explode Action")]
public class ExplodeAction : AiAction
{
    [SerializeField] int damage;
    [SerializeField] float radius;
    NpcCombatController combat;
    NpcMovementController movement;
    Animator animator;
    bool telegraphFinished;
    bool explodeAniFinished;
   
    public override void SetupAction(StateController controller)
    {
        combat = controller.GetCombatController();
        movement = controller.GetMovementController();
        animator = controller.GetAnimator();
        telegraphFinished = false;
        explodeAniFinished = false;
        controller.GetComponent<NpcStats>().Harm(1000);

        GameObject cloud = Instantiate(combat.VFX[1], controller.transform.position, combat.VFX[1].transform.rotation, null);
        
        cloud.gameObject.SetActive(true);
        combat.PlayVFX("On,2");
    }

    public override void Act(StateController controller)
    {
        movement.StopMoving();
        if (telegraphFinished)
        {
            Explode(controller);
        }

        //Delete object
        if (explodeAniFinished)
        {
            Destroy(controller.gameObject);
        }
    }

    void Explode(StateController controller)
    {

        //Collider[] hitColliders = Physics.OverlapSphere(controller.transform.position, radius); //Find each object with a collider within radius
        ////Damage each applicable object.
        //foreach (var collider in hitColliders)
        //{
        //    combat.AttackCollisionDetected(collider.gameObject, damage);
        //}
        ////Do Animator stuff
        //animator.SetBool("Explode", true);
    }

    public void TelegraphFinished()
    {
        telegraphFinished = true;
    }

    public void ExplodeAnimationFinished()
    {
        explodeAniFinished = true;
    }
}
