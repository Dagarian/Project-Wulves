using UnityEngine;

[CreateAssetMenu(fileName = "BehaviourProfile", menuName = "NPC Behaviour Profile")]
public class CreatureProfile : ScriptableObject
{
    [Space(20)]
    [Header("Combat Variables:")]
    [Tooltip("Aggro range in metres.")]
    [Range(1, 50)]
    [SerializeField] float _aggroRange;
    [Tooltip("Attack Range for creature")]
    [Range(1, 30)]
    [SerializeField] float _attackRange;
    [Tooltip("Attack Rate, in seconds.")]
    [SerializeField] float _attackSpeed;
    [Tooltip("Base Damage of creature.")]
    [SerializeField] int _baseDamage;
    [Tooltip("Max distance NPC will travel from default area(s). Set to 0 for no max distance.")]
    [Range(0, 100)]
    [SerializeField] float _leashDistance;

    [Space(10)]
    [SerializeField] int _health;
    [SerializeField] int _armour;
    [SerializeField] int _despawnTime;

    public float LeashDistance { get => _leashDistance; set => _leashDistance = value; }
    public float AggroRange { get => _aggroRange; set => _aggroRange = value; }
    public float AttackRange { get => _attackRange; set => _attackRange = value; }
    public float AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public int BaseDamage { get => _baseDamage; set => _baseDamage = value; }

    public int Health { get => _health; set => _health = value; }
    public int Armour { get => _armour; set => _armour = value; }
    public int DespawnTime { get => _despawnTime; set => _despawnTime = value; }

}
