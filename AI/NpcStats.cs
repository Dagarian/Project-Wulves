using UnityEngine;
using UnityEngine.Events;

public class NpcStats : CreatureStats
{
    [SerializeField] int despawnTimer;
    [SerializeField] int npcID;
    [SerializeField] HealthBar healthBar;
    [SerializeField] int maxHP;
    [SerializeField] int armour;

    // UnityEvent that objects inheriting from NpcStats can subscribe to
    [HideInInspector] public UnityEvent OnHitL = new UnityEvent();  // Triggers on Light attacks
    [HideInInspector] public UnityEvent OnHitH = new UnityEvent();  // Triggers on Heavy attacks

    GameObject spawn;
    public int hp;
    public bool boss;
    bool dead;
    bool inCombat;
    float deathTime;
    Animator ani;
    public int currentPhase = 1;
    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        inCombat = false;
        hp = maxHP;
        ani = GetComponentInChildren<Animator>();

        NpcManager npcManager = GetComponent<NpcManager>();
        if (npcManager != null)
        {
            if (npcManager.profile != null)
            {
                CreatureProfile profile = npcManager.profile;
                despawnTimer = profile.DespawnTime;
                maxHP = profile.Health;
                armour = profile.Armour;
                hp = maxHP;
            }
            healthBar.SetMaxHealth(maxHP);
        }
    }

    private void Update()
    {
        if (boss)
        {
            if (currentPhase == 1 && hp <= maxHP / 2)
            {
                ani.SetBool("Swap Phase", true);
                currentPhase++;
            }
            else
            {
            }
            if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Phase"))
            {
                ani.SetBool("Swap Phase", false);
                ani.SetFloat("Phase", currentPhase);
            }
        }
    }

    //Reduce Creature hp by param dmg.
    public void Harm(int dmg)
    {
        int dmgTaken;
        if (hp > 0)
        {
            if (!ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Phase"))
            {
                if (!ani.GetBool("Blocking") && !ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Taken Hit"))
                {
                    dmgTaken = dmg - armour;
                }
                else
                {
                    dmgTaken = (dmg - armour) / 2;
                }
            }
            else
            {
                dmgTaken = 0;
            }
            if (dmgTaken > 0)
            {
                hp -= dmgTaken;
                if (hp < 0)
                {
                    hp = 0;
                }
                healthBar.SetHealth(hp); //Update UI
                //GetComponentInChildren<Animator>().SetBool("Damaged", false);
                GetComponent<NpcCombatController>().SetTakenHit(true);
                CheckDeath();
                Debug.Log(gameObject.name + " has taken " + dmgTaken + " damage. " + hp + "hp remaining.");
            }
        }
    }

    public void Heal(int heal)
    {
        if (hp + heal > maxHP)
        {
            hp = maxHP;
        }
        else
        {
            hp += heal;
        }
        //Debug.Log(gameObject.name + " has received " + heal + " healing.");
        healthBar.SetHealth(hp);
    }

    public void CheckDeath()
    {
        //If health 0, die.
        if (hp == 0)
        {
            Dead();
        }
        else
        {
            ani.SetBool("Damaged", true);
        }
    }

    private void Dead()
    {
        //Die

        Debug.Log("NPC STATS DEAD");
        dead = true;
        deathTime = Time.time;
        ResetSpawnTimer();
        GetComponent<StateController>().Dead();
        GetComponent<NpcManager>().Dead();
        GetComponent<NpcMovementController>().enabled = false;
        //GetComponent<NpcCombatController>().enabled = false;
        gameObject.tag = "Dead";
        GetComponent<Collider>().enabled = false;
    }

    public int GetMax()
    {
        return maxHP;
    }

    public int GetCurrent()
    {
        return hp;
    }

    //For Usage in sub-classes.
    public bool GetDead()
    {
        return dead;
    }

    public float GetTimeSinceDeath()
    {
        return Time.time - deathTime;
    }

    public bool GetCombatStatus()
    {
        return inCombat;
    }

    public void SetCombatStatus(bool input)
    {
        inCombat = input;
    }
    public bool TimeToDestroy()
    {
        if (GetTimeSinceDeath() > despawnTimer)
        {
            return true;
        }
        return false;
    }

    public void ResetSpawnTimer()
    {
        GameObject spawn = GetSpawn();
        if (spawn != null)
        {
            SpawnProperties spawnProperties = spawn.GetComponent<SpawnProperties>();
            if (spawnProperties != null)
            {
                SpawnController spawnController = spawnProperties.GetController();

                if (spawnController.GetWaitForDeath())
                {
                    //Debug.Log("Reset Timer.");
                    spawnController.ResetTimer(); //Reset spawn timer on own spawner after death.
                }
            }
            else
            {
                //Debug.Log("Spawn has no properties");
            }
        }

        else
        {
            //Debug.Log("controller null");
        }
    }

    public int GetID()
    {
        return npcID;
    }

    public GameObject GetSpawn()
    {
        return spawn;
    }

    public void SetSpawn(GameObject input)
    {
        spawn = input;
    }

    public void SetHealth(int setHealth)
    {
        maxHP = setHealth;
        hp = maxHP;
    }
}
