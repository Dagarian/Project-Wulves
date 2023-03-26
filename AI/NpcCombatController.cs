using PlayerSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcCombatController : MonoBehaviour
{
    [SerializeField] string[] targetTags;

    [SerializeField] List<GameObject> colliders = new List<GameObject>();
    [SerializeField] public List<GameObject> VFX = new List<GameObject>();
    public List<GameObject> telegraphs = new List<GameObject>();
    [SerializeField] List<GameObject> spawnables = new List<GameObject>();

    GameObject currentTarget;
    float currentTargetDistance;

    float timeSinceLastAttack;
    float attackSpeed; //How often (in seconds) the creature attacks
    int basicAttackDamage;
    float attackRange; //This is the default range for all attacks. (Varying attack range to be added)
    float aggroRange;

    NpcStats stats;
    NpcMovementController move;
    Animator animator;
    public float attackAnimationTime;
    public int attackNumber;
    public List<bool> showTelegraph = new List<bool>();
    float hitTime;
    float hitTime2;
    int hitCount;
    bool targetHit;
    float targetHitTimer;
    public bool uninterruptible;
    float timeSinceInterrupt;
    int selectedIndicator;
    float blockTimer;
    float archerTimer;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        move = GetComponent<NpcMovementController>();
        stats = GetComponent<NpcStats>();
        if (attackNumber == 0)
        {
            attackNumber = telegraphs.Count;
        }
        timeSinceLastAttack = 100;
    }

    void Update()
    {
        animator.SetBool("Interupt", uninterruptible);
        animator.SetInteger("HitCount", hitCount);

        if (GetTakenHit())
        {
            hitTime += Time.deltaTime;
            if (hitTime > 0.5f)
            {
                SetTakenHit(false);
                hitTime = 0;
            }
        }

        if (hitCount > 0)
        {
            hitTime2 += Time.deltaTime;
            if (hitTime2 > 2)
            {
                hitCount = 0;
                hitTime2 = 0;
            }
        }

        if (hitCount > 1)
        {
            uninterruptible = true;
            //timeSinceInterupt = 0;
        }
        //else
        //{
        //    timeSinceInterupt += Time.deltaTime;
        //}

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attacking"))
        {
            timeSinceLastAttack = 0;
            uninterruptible = false;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Taken Hit"))
        {
            move.FaceTarget();
        }
        if (Player.playerDead)
        {
            SetTags(null);
            SetTarget(null);
            this.enabled = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Telegraph"))
        {
            timeSinceLastAttack = 0;
            move.facingTarget = false;
            if (telegraphs.Count > 0)
            {
                telegraphs[0].transform.parent.gameObject.SetActive(true);
                Transform indicator = telegraphs[selectedIndicator].transform.GetChild(1);
                indicator.localScale = Vector3.Lerp(indicator.localScale, telegraphs[selectedIndicator].transform.GetChild(0).localScale, (animator.speed + animator.GetCurrentAnimatorClipInfo(0).Length) * Time.deltaTime);
            }
        }
        else
        {
            telegraphs[0].transform.parent.gameObject.SetActive(false);
        }

        if (targetHit)
        {
            animator.SetBool("TargetHit", true);
            targetHitTimer += Time.deltaTime;
            if (targetHitTimer > 3)
            {
                animator.SetBool("TargetHit", false);
                targetHit = false;
                targetHitTimer = 0;
            }
        }

        if (!animator.GetBool("Attacking"))
        {
            timeSinceLastAttack += Time.deltaTime;
            move.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            if (stats.GetID() == 4)
            {
                archerTimer = 0;
            }
        }
        else
        {
            move.GetAgent().obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            move.GetAgent().velocity = Vector3.zero;
            if (stats.GetID() == 4)
            {
                if (archerTimer <= 1.25f)
                {
                    archerTimer += Time.deltaTime;
                    move.FaceTarget();
                }
            }
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Post-attack Emote"))
        {
            if (VFX.Count > 0)
            {
                VFX[0].gameObject.SetActive(false);
            }
        }

        if (animator.GetBool("Blocking"))
        {
            blockTimer += Time.deltaTime;
            if (blockTimer > 1)
            {
                animator.SetBool("Blocking", false);
                blockTimer = 0;
            }
        }
    }

    public void AttackCollisionDetected(GameObject target)
    {
        AttackCollisionDetected(target, basicAttackDamage);
    }


    public void AttackCollisionDetected(GameObject target, int damage)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool attackAnimationPlaying = info.IsName("Attacking");
        string targetTag = target.tag;
        if (targetTags.Contains(targetTag))
        {
            if (targetTag == "Player")
            {
                //Debug.Log("Target hit with tag: Player");
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    player.DamagePlayer(damage);
                    if (!player.GetComponent<Animator>().GetBool("Blocking") && !player.GetComponent<Animator>().GetBool("Invincible"))
                    {
                        targetHit = true;
                    }
                }
            }
            if (targetTag == "HostileNpc")
            {
                //Debug.Log("Target hit with tag: HostileNpc");
                NpcStats stats = target.GetComponent<NpcStats>();
                if (stats != null)
                {
                    stats.Harm(damage);
                    targetHit = true;
                }
            }
            if (targetTag == "Destructible" || targetTag == "NpcDestructible")
            {
                //Debug.Log("Target hit with tag: Destructible or NpcDestructible");
                EnvironmentStats stats = target.GetComponent<EnvironmentStats>();
                if (stats != null)
                {
                    //Debug.Log("Stats not null");
                    stats.Harm(damage);
                    targetHit = true;
                }
            }
        }
    }

    //Finds closest target.
    public GameObject FindClosestTargetWithTag()
    {
        GameObject closestTargetWithTag = null;
        float closestTargetDistance = Mathf.Infinity;
        //For each tag, see if there are any valid targets.
        //For each target, if closer than current target, set to new target.
        if (targetTags != null)
        {
            foreach (var tag in targetTags)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
                //For targets within aggro range, find the closest. (Can be changed to highest aggro later)
                //Debug.Log("Possible targets, checking range");//Get list of targets with current tag from targetTags list.
                if (targets.Length > 0)
                {
                    foreach (var newTarget in targets)
                    {
                        float newTargetDistance = Vector3.Distance(GetPos(), newTarget.transform.position);
                        if (newTargetDistance < closestTargetDistance)
                        {
                            if (newTarget.activeSelf)
                            {
                                closestTargetWithTag = newTarget;
                                closestTargetDistance = newTargetDistance;
                            }
                        }
                    }
                }
            }
        }
        return closestTargetWithTag;
    }

    //Returns a bool showing if current target is within attack range.
    public bool WithinAttackRange()
    {
        if (currentTargetDistance <= attackRange && move.facingTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
        //return currentTargetDistance <= attackRange;
    }

    public void SetupAnimator()
    {
        float attackAnimationSpeedMultiplier = attackAnimationTime / attackSpeed;
        animator.SetFloat("AniAttackSpeed", attackAnimationSpeedMultiplier);
    }

    public bool GetTakenHit()
    {
        return animator.GetBool("Damaged");
    }

    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }

    public void SetTakenHit(bool hit)
    {
        animator.SetBool("Damaged", hit);
        if (hit)
        {
            hitCount++;

            //if (gameObject.name == "Skeleton Soldier")
            //{
            //    BackDash();
            //}
        }
    }

    public void SetTags(string[] tags)
    {
        targetTags = tags;
    }

    public void SetAttackSpeed(float rate)
    {
        attackSpeed = rate;
    }

    public void SetBaseDamage(int damage)
    {
        basicAttackDamage = damage;
    }

    public void SetAttackRange(float range)
    {
        attackRange = range;
    }

    public void SetAggroRange(float range)
    {
        aggroRange = range;
    }

    public void SetTarget(GameObject target)
    {
        currentTarget = target;

        if (currentTarget) // Can make this so enemies can't use certain attacks if they're hitting an objective for example.
        {

            if (currentTarget.tag != "Player")
            {
                animator.SetFloat("AttackType", 1);
            }

        }
    }

    public Vector3 GetPos()
    {
        return gameObject.transform.position;
    }

    public GameObject GetTarget()
    {
        return currentTarget;
    }

    public float GetTargetDistance()
    {
        if (currentTarget != null)
        {
            return currentTargetDistance = Vector3.Distance(GetPos(), currentTarget.transform.position); ;
        } else
        {
            return currentTargetDistance = Mathf.Infinity;
        }
    }

    public float GetTimeSinceAttack()
    {
        return timeSinceLastAttack;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAggroRange()
    {
        return aggroRange;
    }

    public string[] GetTargetTags()
    {
        return targetTags;
    }

    #region Animation & VFX

    public void TelegraphVFX(string telegraph) // Show attack cone/circle
    {
        string[] split = telegraph.Split(',');

        if (split[0] == "On")
        {
            if (showTelegraph[(int.Parse(split[1]))])
            {
                telegraphs[(int.Parse(split[1]))].gameObject.SetActive(true);
                selectedIndicator = int.Parse(split[1]);
            }
            if (split.Length > 2)
            {
                animator.SetFloat("AniTelegraphSpeed", float.Parse(split[2]));
                animator.SetFloat("AniAttackSpeed", float.Parse(split[3]));
            }
        }
        else if (split[0] == "Off")
        {
            if (split[1] != "All")
            {
                telegraphs[(int.Parse(split[1]))].gameObject.SetActive(false);
                telegraphs[(int.Parse(split[1]))].transform.GetChild(1).localScale = new Vector3(0, 0.01f, 0);
                //if (split.Length > 2)
                //{
                //    animator.SetFloat("AniAttackSpeed", float.Parse(split[2]));
                //}
            }
            else
            {
                for (int i = 0; i < telegraphs.Count; i++)
                {
                    telegraphs[i].gameObject.SetActive(false);
                    telegraphs[i].transform.GetChild(1).localScale = new Vector3(0, 0.01f, 0);
                }
            }
        }
    }

    public void PlayVFX(string vfx) // Play VFX (snow hits and such)
    {
        string[] split = vfx.Split(',');

        if (split[0] == "On")
        {
            VFX[(int.Parse(split[1]))].gameObject.SetActive(true);
        }
        else if (split[0] == "Off")
        {
            VFX[(int.Parse(split[1]))].gameObject.SetActive(false);
        }
    }

    public void HitCollider(string collider) // for weapon or arms
    {
        string[] split = collider.Split(',');

        if (split[0] == "On")
        {
            colliders[(int.Parse(split[1]))].gameObject.SetActive(true);
        }
        else if (split[0] == "Off")
        {
            colliders[(int.Parse(split[1]))].gameObject.SetActive(false);
        }
    }

    public bool TargetHit() // Detect if target hit
    {
        if (targetHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PushNPC(string movement) // NOT FINISHED
    {
        if (enabled)
        {
            string[] split = movement.Split(',');

            if (split[0] == "Forward")
            {
                GetComponent<NpcMovementController>().GetAgent().Move(transform.forward * float.Parse(split[1]));
            }
            else
            {
                GetComponent<NpcMovementController>().GetAgent().Move(-transform.forward * float.Parse(split[1]));
            }
            animator.SetBool("Dodge", false);
        }
    }

    public void SpawnProjectile(string spawn)
    {
        string[] split = spawn.Split(',');

        if (spawnables.Count > 0)
        {
            GameObject projectile = Instantiate(spawnables[int.Parse(split[0])], transform.position + (transform.up * 1.75f), transform.rotation, null);

            projectile.GetComponent<Projectile>().damage = basicAttackDamage;
            projectile.GetComponent<Projectile>().speed = float.Parse(split[1]);
            projectile.GetComponent<Projectile>().timeAlive = float.Parse(split[2]);
            if (int.Parse(split[3]) != 0)
            {
                projectile.GetComponent<Projectile>().headTowards = telegraphs[2].transform.position;
            }
        }
    }

    public void Throw(int attack)
    {
        // Set collider and indicator position to player's current position.
        colliders[attack].transform.position = currentTarget.transform.position;
        telegraphs[attack].transform.position = currentTarget.transform.position;
        VFX[attack].transform.position = currentTarget.transform.position;
    }

    #endregion
}
