using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    [SerializeField] State remainState; //A meta state used to keep state in current state.
    [SerializeField] State defaultState;
    [SerializeField] State currentState;
    public State[] multiState;
    NpcStats stats;
    NpcCombatController combat;
    NpcMovementController movement;
    NavMeshAgent agent;
    Animator anima;
    bool active;
    float stateTime;
    bool isDead;
    bool manualTrigger;
    bool objectiveEnemy;

    private void Awake()
    {
        currentState = defaultState;
        isDead = false;
        combat = GetComponent<NpcCombatController>();
        movement = GetComponent<NpcMovementController>();
        stats = GetComponent<NpcStats>();
        agent = GetComponent<NavMeshAgent>();
        anima = GetComponent<Animator>();
        if (currentState != null)
        {
            currentState.SetupAction(this);
        }
    }

    public void SetupAi(bool activationInput)
    {
        active = activationInput;
        if(active)
        {
            movement.enabled = true;
            combat.enabled = true;
            agent.enabled = true;
        } else
        {
            movement.enabled = false;
            combat.enabled = false;
            agent.enabled = false;

            if (combat.telegraphs.Count > 0)
            {
                for (int i = 0; i < combat.telegraphs.Count; i++)
                {
                    combat.telegraphs[i].SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        if (!active)
        {
            CheckDestroyAfterDeath();
        }
        currentState.UpdateState(this);

        if (currentState.name == "T1_ObjectiveSpawnState" ||
            currentState.name == "T2_ObjectiveSpawnState" ||
            currentState.name == "T3_ObjectiveSpawnState" ||
            currentState.name == "Boss_ObjectiveSpawnState")
        {
            objectiveEnemy = true;
        }
    }


    public void TransitionToState(State nextState)
    {
        if (nextState != null)
        {
            if (nextState.name == "Default")
            {
                //Debug.Log("Transition to state " + nextState.name);
                currentState = defaultState;
                currentState.SetupAction(this);
                //Debug.Log("StateController-side Setup: " + currentState.name);
                OnExitState();
            }
            else if (nextState != remainState)
            {
                //Debug.Log("Transition to state " + nextState.name);
                currentState = nextState;
                currentState.SetupAction(this);
                //Debug.Log("StateController-side Setup: " + currentState.name);
                OnExitState();
            }
        }
        if(manualTrigger)
        {
            manualTrigger = false;
        }
    }

    public void AnimationState(string s)
    {
        State state = (State)ScriptableObject.CreateInstance(s);

        if (this.gameObject.name == "Ice Golem(Clone)")
        {
            switch (s)
            {
                case "T3_AttackingMotionState":

                    break;

                case "T3_AttackPerformedEmoteState":

                    break;

                case "T3_DeathState":

                    break;

                case "T3_IdleState":

                    break;

                case "T3_TakenHitState":

                    break;

                case "T3_TelegraphingAttackState":

                    break;

                case "T3_ThreatenedState":

                    break;

                case "T3_WalkToPlayerState":

                    break;

                default:
                    break;
            }
        }

        TransitionToState(state);
    }

    public void Dead()
    {
        
        isDead = true;
        DeadTrigger dead = (DeadTrigger) ScriptableObject.CreateInstance("DeadTrigger");
        if (dead.TriggerState(this))
        {
            if (currentState.name != "Death")
            {
                State deathState = (State)ScriptableObject.CreateInstance("State");
                deathState.name = "Death";
                deathState.actions = new AiAction[1];
                deathState.actions[0] = (DeathAction)ScriptableObject.CreateInstance("DeathAction");
                TransitionToState(deathState);
            }
        }
    }

    private void CheckDestroyAfterDeath()
    {
        //Debug.Log(stats.TimeToDestroy() + ": Time to destroy");
        if (stats.TimeToDestroy())
        {
            //Debug.Log(name + " object has been destroyed.");
            Destroy(gameObject);
        }
    }

    public bool CheckIfCountdownElapsed(float duration)
    {
        stateTime += Time.deltaTime;
        return (stateTime >= duration);
    }


    public bool FinishedTelegraph()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Telegraph"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool FinishedAttacking()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attacking"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FinishedSpawning()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Spawning"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FinishedEmoting()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Post-attack Emote"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FinishedBlocking()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Blocking"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FinishedDodging()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dodging"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool FinishedPhasing()
    {
        if (anima.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Phase"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ObjectiveEnemy()
    {
        return objectiveEnemy;
    }
    public bool SwapPhase()
    {
        return anima.GetBool("Swap Phase");
    }

    private void OnExitState()
    {
        stateTime = 0;
    }

    public NpcMovementController GetMovementController()
    {
        return movement;
    }

    public NpcCombatController GetCombatController()
    {
        return combat;
    }

    public NpcStats GetStats()
    {
        return stats;
    }

    public Animator GetAnimator()
    {
        return anima;
    }

    public bool GetManualTrigger()
    {
        return manualTrigger;
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    public State GetMultiState(int index)
    {
        return multiState[index];
    }

    public void SetActive(bool input)
    {
        active = input;
    }

    public void SetDefaultState(State newState)
    {
        defaultState = newState;
        currentState = defaultState;
    }

    public void ManualTrigger()
    {
        manualTrigger = true;
    }
}
