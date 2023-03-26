using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class SpawnController : MonoBehaviour
{
    [Tooltip("The max number of seconds before starting spawns.")]
    public float spawnDelayMax;
    
    [Tooltip("The minimum number of seconds before starting spawns.")]
    public float spawnDelayMin;
    
    [Tooltip("The rate at which things are spawned in seconds")]
    [SerializeField] float spawnRate;

    [Tooltip("The max number of individual spawns. 0 = Off.")]
    [SerializeField] float maxSpawns = 0;

    [Tooltip("A new default state that will be set to all spawned NPCs from this spawner. Leave blank to disregard.")]
    [SerializeField] State defaultState;

    [Tooltip("If true, check for death of specific spawn's NPC before spawning another.")]
    [SerializeField] bool waitForDeath;

    [Tooltip("If true, spawns NPCs at all spawn locations, otherwise one NPC at one random spawn location.")]
    [SerializeField] bool waveSpawn;
    
    [Tooltip("The size of a single wave. Only used if waveSpawn is true.")]
    public int waveSize;
    
    [Tooltip("The time between spawning each game object in a wave.")]
    public float waveStagger;

    [Tooltip("If true, will choose a random point to spawn at, otherwise will spawn in order.")] 
    [SerializeField] bool spawnAtRandomPoint;
    
    [Tooltip("If true, rather than waiting for full wave to die, will 'fill' the wave 1(prefab) at a time.")]
    [SerializeField] bool fillWave;

    [Tooltip("The delay in seconds between filling a wave spot.")]
    public float fillWaveDelay = 0;
    float deltaSpawnTime; //The next time to spawn something.

    [Tooltip("Array of spawn points, chosen randomly if wave false.")]
    [SerializeField] GameObject[] spawnPoints;

    [Tooltip("Array of NPCs to spawn. Always chosen randomly if length > 1.")]
    [SerializeField] GameObject[] npcsToSpawn;

    //Can add manual index choice if needed.
    [Tooltip("If true, will use random index for spawning NPC. Else spawn NPC at index 0")] 
    [SerializeField] private bool spawnRandomNpc;

    [Tooltip("An array of points to move to. Not needed for Camper. Attack behavior will pull from index 0. Patrol will move in order between all. Applies to all spawns.")]
    [SerializeField] Transform[] defaultLocations;


    [Tooltip("A list of tags to replace the NPC's default. Stays default if length == 0.")]
    [SerializeField] string[] newTargetTags;

    [SerializeField] GameObject enemyParent;

    private float startTime;
    private float spawnDelay;
    private bool spawned;
    private int spawnIndex;
    private int spawnCount;
    private int aliveCount;
    private float despawnTimer;
    private int waveSpawnIndex;
    private bool waveSpawned; //For use with FillWave to see if initial wave has been spawned.
    private bool waveFilled;
    
    private void Awake()
    {
        startTime = Time.time;
        spawnDelay = Random.Range(spawnDelayMin, spawnDelayMax);
        if (newTargetTags.Length > 0)
        {
            foreach (var npc in npcsToSpawn)
            {
                npc.GetComponent<NpcCombatController>().SetTags(newTargetTags);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        waveFilled = false;
        spawned = false;
        spawnCount = 0;
        waveSpawned = false;
        waveSpawnIndex = 0;
        if(fillWaveDelay < 0)
        {
            fillWaveDelay = 0;
        }
        ResetTimer(); //Initialise timer
        foreach (var spawnLocation in spawnPoints)
        {
            spawnLocation.AddComponent(typeof(SpawnProperties));
            spawnLocation.GetComponent<SpawnProperties>().SetController(gameObject.GetComponent<SpawnController>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        float timeElapsed = Time.time - startTime;
        if (timeElapsed < spawnDelay) return;
        if(enemyParent != null)
        {
            aliveCount = enemyParent.transform.childCount;
            //Debug.Log("Enemies Alive: " + aliveCount);
        }
        else
        {
            Debug.LogError("No enemyParent Set.");
        }
        // Check to see if there are NPCs to spawn && within max spawn time if set.
        if (npcsToSpawn.Length > 0 && (spawnCount < maxSpawns || maxSpawns == 0)) 
        {
            int npcIndex = 0;
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randomSpawnIndex]; //Choses random spawn point for non-wave spawning
            bool spawn = !(spawnCount >= maxSpawns); //If spawn count >= max spawns, spawn = false.

            //If waitForDeath, check for deaths of all members.
            if (waitForDeath)
            {
                //Check for full wave if waveSpawn else check for single npc at spawn.
                if (waveSpawn)
                {
                    if (fillWave)
                    {
                        if (aliveCount >= waveSize)
                        {
                            spawn = false;
                            ResetTimer();
                        }
                    }
                    else
                    {
                        if (aliveCount > 0)
                        {
                            spawn = false;
                            ResetTimer();
                        }
                    }
                }
                else
                {
                    if (spawnPoint.GetComponent<SpawnProperties>().GetNpc() != null)
                    {
                        spawn = false;
                        ResetTimer();
                    }
                }
            }
            if (spawn)
            {
                Debug.Log("fillWave: " + fillWave + ". waveSpawned: " + waveSpawned);
                if (fillWave && waveSpawned)
                {
                    Debug.Log("aliveCount: " + aliveCount + ". waveSize: " + waveSize);
                    if (aliveCount < waveSize)
                    {
                        Debug.Log("Filling Wave.");
                        waveFilled = false;
                        StartCoroutine(FillWave(fillWaveDelay));
                    }
                }
                else
                {
                    if (Time.time >= deltaSpawnTime)
                    {
                        if (spawnRandomNpc)
                        {
                            npcIndex = Random.Range(0, npcsToSpawn.Length);
                        }

                        if (waveSpawn)
                        {
                            StartCoroutine(SpawnWave(npcIndex, spawnPoints));
                        }
                        else //Not Wave spawn
                        {
                            SpawnNpc(npcIndex, spawnPoint);
                        }
                        ResetTimer();
                    }
                }
            }
        }
        else
        {
            despawnTimer += Time.deltaTime;
            if (despawnTimer > 3)
            {
                if (transform.childCount > 0)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (transform.GetChild(i).childCount > 0)
                        {
                            if (transform.GetChild(i).GetChild(0).gameObject.activeInHierarchy)
                            {
                                if (transform.GetChild(i).GetChild(0).GetComponent<Animator>())
                                {
                                    transform.GetChild(i).GetChild(0).GetComponent<Animator>().SetBool("SpawnerOff", true);
                                }
                            }
                        }
                    }
                }
                this.enabled = false;
            }
        }
    }

    private IEnumerator FillWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!waveFilled)
        {
            bool filled = false;
            int randomNpcIndex = Random.Range(0, npcsToSpawn.Length);
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
            SpawnNpc(randomNpcIndex, spawnPoints[randomSpawnIndex]);
        }
        waveFilled = true;
        
    }
    /* Params:
     * index: Index of NPC in GameObject[] npcsToSpawn. Set to -1 for random index.
     * spawnList: The transform.position of GameObject spawn is the spawn location.
     */
    private IEnumerator SpawnWave(int indexInput, GameObject[] spawnList)
    {
        //Debug.Log("SpawnWave() called");
        waveSpawned = false;
        int npcIndex = indexInput;
        int spawnPointIndex = 0;
        while (!waveSpawned)
        {
            if (waveSpawnIndex == waveSize || spawnCount == maxSpawns)
            {
                waveSpawned = true;
                waveSpawnIndex = 0;
                Debug.Log("Wave Spawned.");
                break;
            }
            if (spawnAtRandomPoint)
            {
                spawnPointIndex = Random.Range(0, spawnList.Length);
            }
            else
            {
                if (spawnPointIndex + 1 > spawnList.Length - 1)
                {
                    spawnPointIndex = 0;
                }
                else
                {
                    spawnPointIndex++;
                }
            }
            if (indexInput == -1)
            {
                npcIndex = Random.Range(0, npcsToSpawn.Length);
            }
            //Debug.Log("Before SpawnNpc(); call.");
            SpawnNpc(npcIndex, spawnList[spawnPointIndex]);
            Debug.Log("Wave spawned 1 NPC.");
            waveSpawnIndex++;
            yield return new WaitForSeconds(waveStagger);
        }
    }

    //Spawns an object at the given index from the Npcs list, at a given spawn point.
    void SpawnNpc(int index, GameObject spawn)
    {
        if (!enemyParent)
        {
            if (defaultState)
            {
                if (defaultState.name.ToString() == "Camp")
                {
                    enemyParent = GameObject.Find("Placed").transform.GetChild(0).gameObject;
                }
                if (defaultState.name.ToString() == "Patrol")
                {
                    enemyParent = GameObject.Find("Placed").transform.GetChild(1).gameObject;
                }
            }
            else if (!defaultState)
            {
                enemyParent = GameObject.Find("Placed").transform.GetChild(0).gameObject;
            }
        }
        GameObject npc = Instantiate(npcsToSpawn[index], spawn.transform.position, Quaternion.identity, enemyParent.transform);
        npc.name = npcsToSpawn[index].name;
        NpcMovementController movement = npc.GetComponent<NpcMovementController>();
        NpcCombatController combat = npc.GetComponent<NpcCombatController>();
        NpcStats status = npc.GetComponent<NpcStats>();
        StateController stateController = npc.GetComponent<StateController>();
        combat.SetTags(newTargetTags);
        spawn.GetComponent<SpawnProperties>().SetNpc(npc);
        status.SetSpawn(spawn);
        movement.RefreshSpawn();
        if (defaultState != null)
        {
            stateController.SetDefaultState(defaultState);
            stateController.TransitionToState(defaultState);
        }
        if (defaultLocations.Length > 0)
        {
            movement.SetDefaultLocations(defaultLocations);
        }
        if (transform.GetComponent<Hunted>())
        {
            npc.AddComponent<Hunted>();
        }
        spawnCount++;
    }

    public void ResetSpawnCount()
    {
        spawnCount = 0;
    }

    public void ResetTimer()
    {
        deltaSpawnTime = Time.time + spawnRate;
    }

    public bool GetWaitForDeath()
    {
        return waitForDeath;
    }

    public void SetMaxSpawns(float spawns)
    {
        maxSpawns = spawns;
    }

    public void SetSpawnPoints(GameObject[] spawns)
    {
        spawnPoints = spawns;
    }
    
    public float GetSpawnCount()
    {
        return spawnCount;
    }

    public void SetNpcToSpawn(GameObject npc)
    {
        npcsToSpawn[0] = npc;
    }
}
