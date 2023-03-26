using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Circle Around Target")]
public class CircleAroundTargetAction : AiAction
{
    [SerializeField] float minRange;
    [SerializeField] float maxRange;
    [SerializeField] float movementMultiplier;
    [SerializeField] float angleTheta;
    NpcCombatController combat;
    NpcMovementController movement;
    GameObject target;
    public override void SetupAction(StateController controller)
    {
        combat = controller.GetCombatController();
        movement = controller.GetMovementController();
        movement.ContinueMoving();
        movement.SetAutoBraking(false);
    }
    public override void Act(StateController controller)
    {
        target = combat.GetTarget();
        Transform targetPosition = target.transform;
        Transform myPosition = controller.transform;
        Vector3 moveToLocation;
        float radius = Vector3.Distance(targetPosition.position, myPosition.position);
        float currentAngle = (Mathf.Atan2(targetPosition.position.z - myPosition.position.z, targetPosition.position.x - myPosition.position.x) * Mathf.Rad2Deg) + 180;
        float nextAngleRad = (currentAngle + angleTheta) * Mathf.Deg2Rad;
        //Debug.Log("Current Angle(rad): " + (currentAngle) + ". Next Angle(rad):" + nextAngleRad * Mathf.Rad2Deg);
        if (radius > minRange && radius < maxRange)
        {
            //Debug.Log("Target is within boundaries, circling.");
            moveToLocation = new Vector3(targetPosition.position.x + radius * Mathf.Cos(nextAngleRad) , targetPosition.position.y, targetPosition.position.z + radius * Mathf.Sin(nextAngleRad)); //Find x & z in reference to target (Polar coordinates)
            movement.SetDestination(moveToLocation);
        }
        else if (radius < minRange)
        {
            //Debug.Log("Target is too close, moving away.");
            moveToLocation = myPosition.position + ((myPosition.position - targetPosition.position) * movementMultiplier);
            movement.SetDestination(moveToLocation);
        }
        else if (radius > maxRange)
        {
            //Debug.Log("Target is too far, moving closer.");
            moveToLocation = myPosition.position - ((myPosition.position - targetPosition.position) * movementMultiplier);
            movement.SetDestination(moveToLocation);
        }
        //Debug.Log("Current Position(x,y,z): " + myPosition);
        //Debug.Log("Moving to(x,y,z): " + moveToLocation);
    }
}
