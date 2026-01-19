using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] setPlace;

    [SerializeField] public float speed = 3f;
    [SerializeField] public float reachDistance = 0.2f;

    [Header("Animation IDs")]
    [SerializeField] public string WalkID = "Walk";
    [SerializeField] public string HurtID = "Hurt";

    [Header("UI Damage Text")]
    [SerializeField] public TMP_Text damageText;

    [Header("Health Bar (UI Image)")]
    [SerializeField] public Image barFill;

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    public bool is_ready;
    public bool is_dead = false;

    [SerializeField] public Animator anim;
    public int spawnIndex;
    public EnemySpawner spawner;

    [SerializeField] public ParticleSystem[] DiedEfect;
    [SerializeField] public GameObject HealthParent;
    [SerializeField] public GameObject TextParent;

    public SpriteRenderer rendererRef;
    public PlayerSpawner attackSpawner;

    [SerializeField] public float footStepInterval = 0.4f;

    public EnemyMovement movementHandler;
    public EnemyHealth healthHandler;
    public EnemyDeath deathHandler;

    public void Init(EnemySpawner spawnerRef, int index, Transform[] paths)
    {
        spawner = spawnerRef;
        spawnIndex = index;
        setPlace = paths;
    }

    void Awake()
    {
        spawner = FindAnyObjectByType<EnemySpawner>();
        attackSpawner = FindAnyObjectByType<PlayerSpawner>();
        rendererRef = GetComponent<SpriteRenderer>();

        // Yardımcı classlar
        deathHandler = new EnemyDeath(this);
        healthHandler = new EnemyHealth(this, deathHandler);
        movementHandler = new EnemyMovement(this);
    }

    void Start()
    {
        healthHandler.Initialize();

        if (attackSpawner != null)
            attackSpawner.RegisterEnemy(this);

        StartCoroutine(movementHandler.MoveCoroutine());
    }

    public void TakeDamage(float dmg)
    {
        healthHandler.TakeDamage(dmg);
    }
}
