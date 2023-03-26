using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Leap Attack")]
public class LeapAttackAction : AiAction
{
    [SerializeField] float timeToLeap;
    [SerializeField] float attackRange;
    [SerializeField] float maxLeapDistance;
    NpcCombatController combat;
    NpcMovementController movement;
    Animator animator;
    GameObject target;
    Vector3 startPos;
    Vector3 moveToLocation;
    Vector3 targetPosition;
    Vector3 myPosition;
    bool leapAttackAnimation;
    bool leapt;
    float localTime;
    public override void SetupAction(StateController controller)
    {
        myPosition = controller.gameObject.transform.position;
        leapt = false;
        localTime = 0;
        leapAttackAnimation = false;
        combat = controller.GetCombatController();
        movement = controller.GetMovementController();
        animator = controller.gameObject.GetComponentInChildren<Animator>();
        animator.SetBool("Walking", false);
        target = combat.GetTarget();
        Debug.Log(controller.gameObject.name + " target is: " + target.name);
        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        Debug.Log("target pos: " + targetPosition);
        startPos = new Vector3(myPosition.x, myPosition.y, myPosition.z);
        float distance = Vector3.Distance(startPos, targetPosition) - attackRange;
        if(distance > maxLeapDistance && maxLeapDistance > 0)
        {
            distance = maxLeapDistance;
        }
        Debug.Log("Distance: " + distance);
        float angle = Mathf.Atan2(targetPosition.z - startPos.z, targetPosition.x - startPos.x); //Angle ranges between 0 - 2Pi Rad
        //Debug.Log("Angle: " + angle * Mathf.Rad2Deg);

        //This currently produces a 'strange' location. Using the same math as seen in "CircleAroundTargetAction". Distance is too far.
        //Double checked "angle" outputs correctly.
        moveToLocation = new Vector3(targetPosition.x + (distance * Mathf.Cos(angle)), targetPosition.y, targetPosition.z + (distance * Mathf.Sin(angle)));
    }
    public override void Act(StateController controller)
    {
        movement.GetAgent().ResetPath();
        if (!leapt)
        {
            Debug.Log("leaping");
            leapAttackAnimation = true;
            if (localTime < timeToLeap)
            {
                float interpValue = localTime / timeToLeap; //Starts at 0 and increases to 1 as time gets closer to total time.
                //Debug.Log("interp: " + interpValue);
                controller.transform.position = Vector3.Lerp(startPos, moveToLocation, interpValue);
                Debug.Log("Start Pos: " + startPos + ".    End Pos: " + moveToLocation + ".    Current Pos: " + myPosition);
                Debug.Log("Distance to target: " + Vector3.Distance(myPosition, moveToLocation));
                localTime += Time.deltaTime;
            }
            else
            {
                myPosition = moveToLocation;
                Debug.Log("Leapt");
                leapAttackAnimation = false;
                leapt = true;
            }
        }
        animator.SetBool("leapAttack", leapAttackAnimation);
    }
}
