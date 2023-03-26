using UnityEngine;


public class NpcManager : MonoBehaviour
{
    NpcMovementController movement;
    NpcCombatController combat;
    public CreatureProfile profile;
    StateController stateController;
    [SerializeField] AudioManager.MultiSlave LocalAudioSlave;

    void Start()
    {
        stateController = GetComponent<StateController>();
        movement = GetComponent<NpcMovementController>();
        combat = GetComponent<NpcCombatController>();
        LoadProfile(profile);
        stateController.SetupAi(true);
        combat.SetupAnimator();

        //Seraphs edit for sudio
        if (LocalAudioSlave!=null)
        {
            print("LocalSlave Error - Seraph says please check prefab");
        }
    }

    private void LoadProfile(CreatureProfile profile)
    {
        combat.SetAggroRange(profile.AggroRange);
        combat.SetAttackRange(profile.AttackRange);
        combat.SetAttackSpeed(profile.AttackSpeed);
        combat.SetBaseDamage(profile.BaseDamage);
        movement.SetLeashDistance(profile.LeashDistance);        
    }

    public void Dead()
    {
        stateController.SetupAi(false);
    }

    public void Revived()
    {
        stateController.SetupAi(true);
    }

    public void PlayAudio(string audio)
    {
        if (LocalAudioSlave!=null)
        {
            string[] split = audio.Split(',');
            int value = int.Parse(split[1]);
            switch (split[0])
            {
                case "Weapon":
                    LocalAudioSlave.PlayAudioSlaveIndex(2, value);//New AudioLine
                    return;
                case "Movement":
                    LocalAudioSlave.PlayAudioSlaveIndex(0, value);//New AudioLine
                    return;
                case "HitBy":
                    LocalAudioSlave.PlayAudioSlaveIndex(1, value);//New AudioLine
                    return;
                case "Extra":
                    LocalAudioSlave.PlayAudioSlaveIndex(3, value);//New AudioLine
                    return;
                default:

                    break;
            }
        }
        else
        {
            print("Local Audio Slave is missing from prefab");
        }
    }
}
