using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Triggers/Have Target")]
//Checks to see if AI still has a target (And that it is valid).
public class HaveTarget : StateTrigger
{
    public override bool TriggerState(StateController controller)
    {
        NpcCombatController combat = controller.GetComponent<NpcCombatController>();
        if (combat.GetTargetTags() != null)
        {
            List<string> validTags = new List<string>(combat.GetTargetTags());
            if (combat.GetTarget() != null && validTags.Contains(combat.GetTarget().tag))
            {
                GameObject target = combat.GetTarget();
                if (target.tag == "Player")
                {
                    if (PlayerSystems.Player.playerDead)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
