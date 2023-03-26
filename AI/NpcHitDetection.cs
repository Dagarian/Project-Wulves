using UnityEngine;

public class NpcHitDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        NpcCombatController combat = GetComponentInParent<NpcCombatController>();
        if (combat.enabled)
        {
            combat.AttackCollisionDetected(collider.gameObject);
            //Debug.Log("Collision! Sending target with tag: " + collider.gameObject.tag);
        }
    }
}
