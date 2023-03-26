using UnityEngine;
using UnityEngine.AI;

public class NpcMovementController : MonoBehaviour
{
    [Tooltip("An array of points to move to. 'attack point' behavior will pull from index 0. Patrol will move in order between all.")]
    [SerializeField] Transform[] defaultLocations;
    int patrolIndex;
    float leashRange;

    NavMeshAgent agent;
    public GameObject currentTarget;
    float targetDistance;
    Vector3 defaultLocation;
    float distanceFromDefault;
    Vector3 destination;
    GameObject spawn;
    NpcCombatController combat;
    NpcStats stats;

    Animator animator;
    bool walking;
    public bool facingTarget;
    RaycastHit hit;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<NpcCombatController>();
        stats = GetComponent<NpcStats>();
        animator = GetComponent<Animator>();
        targetDistance = Mathf.Infinity;
        currentTarget = null;
        patrolIndex = 0;

        //if (spawn == null)
        //{
        //    GameObject newSpawn = new GameObject(gameObject.name + " spawn");
        //    newSpawn.transform.position = GetPos();
        //    spawn = newSpawn;
        //}
        //if (defaultLocations.Length == 0)
        //{
        //    defaultLocations = new Transform[1];
        //    defaultLocations[0] = spawn.transform;
        //}

    }

    private void Start()
    {
        spawn = stats.GetSpawn();
        defaultLocation = defaultLocations[0].position;
        destination = defaultLocation;
        walking = false;
    }


    // Update is called once per frame
    // Check for targets each frame for each tag, if within aggro range, move towards until within attack range.
    void Update()
    {
        UpdateTarget();
        LookingAtTarget();
        if (defaultLocation == null)
        {
            defaultLocations[0] = gameObject.transform;
            defaultLocation = gameObject.transform.position;
        }
        currentTarget = combat.GetTarget();
        if (animator)
        {
            animator.SetBool("Walking", walking);
        }

        if (combat.GetTakenHit())
        {
            StopMoving();
        }

        if (currentTarget != null)
        {
            string stateName = GetComponent<StateController>().GetCurrentState().actions[0].name;
            if (stateName == "Move To Attack")
            {
                SetDestination(currentTarget.transform.position);
                GoToDestination();
            }
        }
    }

    public void ChangePatrolPoint()
    {
        patrolIndex++;
        if (defaultLocations.Length - 1 < patrolIndex)
        {
            patrolIndex = 0;
        }
        destination = defaultLocations[patrolIndex].position;
    }

    //Checks to see if the target is within aggro range of this object, and within leash range (Aggro is second to staying within leash range)

    //Checks to see if this object is within maxLeashDistance of its default location.
    public bool WithinLeashRange()
    {
        if (defaultLocations.Length > 0)
        {
            FindClosestDefaultLocation();
            if (distanceFromDefault < leashRange || leashRange == 0)
            {
                //Debug.Log("Within Leash Range.");
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    //Moves to current destination
    public void GoToDestination()
    {
        string stateName = GetComponent<StateController>().GetCurrentState().actions[0].name;
        agent.isStopped = false;
        if (Arrived())
        {
            walking = false;
            //Debug.Log("Arrived.");
        }
        else if (!agent.pathPending && stateName == "Move To Attack")
        {
            agent.SetDestination(destination);
            walking = true;
            //Debug.Log("Moving to destination.");
        }
    }

    //Returns true if have arrived at destination.
    public bool Arrived()
    {
        float distance = Vector3.Distance(GetPos(), destination);
        float stoppingDistance = agent.stoppingDistance;
        if (stoppingDistance < 2)
        {
            stoppingDistance = 2;
        }
        return distance <= stoppingDistance;
    }

    //This should be used when there is multiple deafault locations to see which location is the closest, rather than using its current destination.
    //It is used to ensure that an AI stays within range anywhere along its designated path.
    public void FindClosestDefaultLocation()
    {
        FindDistanceFromDefault();
        //For each patrol point, set 
        foreach (var location in defaultLocations)
        {
            float currentPointDistance = Vector3.Distance(GetPos(), location.position);
            if (currentPointDistance < distanceFromDefault)
            {
                defaultLocation = location.position;
                distanceFromDefault = currentPointDistance;
            }
        }
    }

    public float FindDistanceFromDefault()
    {
        return distanceFromDefault = Vector3.Distance(GetPos(), defaultLocation);
    }

    public void FaceTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void LookingAtTarget()
    {
        Debug.DrawRay(transform.position + transform.up * 1, transform.forward * 5, Color.red);

        if (Physics.Raycast(transform.position + transform.up * 1, transform.forward * 5, out hit))
        {
            if (hit.transform != null)
            {
                if (hit.collider.tag == "Player")
                {
                    facingTarget = true;
                }
            }
            else
            {
                facingTarget = false;
            }
        }
    }

    public void ContinueMoving()
    {
        if (this.enabled)
        {
            agent.isStopped = false;
            walking = true;
        }
    }

    public void StopMoving()
    {
        if (this.enabled)
        {
            walking = false;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public void RefreshTargetDistance()
    {
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(GetPos(), currentTarget.transform.position);
        }
        else
        {
            targetDistance = Mathf.Infinity;
            //Debug.Log("No target, distance Inf.");
        }
    }

    public void RefreshSpawn()
    {
        spawn = GetComponent<NpcStats>().GetSpawn();
    }

    public void SetDefaultLocations(Transform[] locations)
    {
        defaultLocations = locations;
        defaultLocation = defaultLocations[0].position;
    }

    public void SetDestinationToDefault()
    {
        destination = defaultLocation;
    }

    public void SetDestinationToSpawn()
    {
        RefreshSpawn();
        destination = spawn.transform.position;
    }

    public void SetDefaultToSpawn()
    {
        RefreshSpawn();
        defaultLocation = spawn.transform.position;
    }

    public void SetDestination(Vector3 input)
    {
        destination = input;
    }

    public void SetLeashDistance(float distance)
    {
        leashRange = distance;
    }

    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }

    public void UpdateTarget()
    {
        currentTarget = combat.GetTarget();
    }

    public float GetLeashDistance()
    {
        return leashRange;
    }

    //return own Vector3 position
    public Vector3 GetPos()
    {
        return transform.position;
    }

    public GameObject GetSpawn()
    {
        return spawn;
    }

    public Vector3 GetDefaultLocation()
    {
        return defaultLocation;
    }

    public Transform[] GetDefaultLocations()
    {
        return defaultLocations;
    }

    public Vector3 GetDestination()
    {
        return destination;
    }

    public float GetTargetDistance()
    {
        return targetDistance;
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public void SetAutoBraking(bool input)
    {
        if (agent != null)
        {
            agent.autoBraking = input;
        } else
        {
            Debug.LogError("Agent is null");
        }
    }
}


