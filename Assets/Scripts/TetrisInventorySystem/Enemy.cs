using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    private Transform[] setPlace;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float reachDistance = 0.2f;

    [Header("Animation IDs")]
    [SerializeField] private string WalkID = "Walk";
    [SerializeField] private string HurtID = "Hurt";

    [Header("UI Damage Text")]
    [SerializeField] private TMP_Text damageText;

    [Header("Health Bar (UI Image)")]
    [SerializeField] private Image barFill;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool is_ready;
    public bool is_dead = false;

    [SerializeField] private Animator anim;
    public int spawnIndex;
    private EnemySpawner spawner;

    [SerializeField] private ParticleSystem[] DiedEfect;
    [SerializeField] private GameObject HealthParent;
    [SerializeField] private GameObject TextParent;

    private SpriteRenderer _renderer;

    // 🔥 Saldırıyı yöneten Spawner (item fırlatan)
    private Spawner attackSpawner;
    private float footStepTimer = 0f;
    [SerializeField] private float footStepInterval = 0.4f;
    public void Init(EnemySpawner spawnerRef, int index, Transform[] paths)
    {
        spawner = spawnerRef;
        spawnIndex = index;
        setPlace = paths;
    }

    void Awake()
    {
        spawner = FindAnyObjectByType<EnemySpawner>();
        attackSpawner = FindAnyObjectByType<Spawner>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (damageText != null)
            damageText.gameObject.SetActive(false);

        if (barFill != null)
            barFill.fillAmount = 1f;

        // 🔥 Enemy doğduğunda → saldırı kuyruğuna EKLE
        if (attackSpawner != null)
            attackSpawner.RegisterEnemy(this);

        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Transform target = setPlace[spawnIndex];

        anim.SetBool(WalkID, true);

        while (Vector3.Distance(transform.position, target.position) > reachDistance)
        {
            footStepTimer += Time.deltaTime;

            if (footStepTimer >= footStepInterval)
            {
                footStepTimer = 0f;
                SoundManager.Instance.EnemyFootSound();
            }
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
            yield return null;
        }

        anim.SetBool(WalkID, false);
        is_ready = true;
    }

    public void TakeDamage(float dmg)
    {
        if (is_dead) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
        StartCoroutine(DamageRoutine(dmg));
         SoundManager.Instance.EnemyHurtSound();
        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (barFill != null)
            barFill.fillAmount = currentHealth / maxHealth;
    }

    private IEnumerator DamageRoutine(float dmg)
    {
        anim.SetBool(HurtID, true);

        if (damageText != null)
        {
            damageText.gameObject.SetActive(true);
            damageText.text = "-" + dmg;
        }

        yield return new WaitForEndOfFrame();

        anim.SetBool(HurtID, false);
        yield return new WaitForSeconds(0.1f);

        if (damageText != null)
            damageText.gameObject.SetActive(false);
    }

    private void Die()
    {
        if (is_dead) return;
        is_dead = true;
        SoundManager.Instance.EnemyDieSound();
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // UI kapat
        if (HealthParent != null) HealthParent.SetActive(false);
        if (TextParent != null) TextParent.SetActive(false);

        // Sprite kapat
        if (_renderer != null) _renderer.enabled = false;

        // Particle efektleri başlat
        foreach (var p in DiedEfect)
        {
            if (p != null) p.Play();
        }

        // 🔥 Kuyruktan çıkar (artık hedef alınmasın)
        if (attackSpawner != null)
            attackSpawner.UnregisterEnemy(this);

        // Eski sistemin respawn mantığı:
        if (spawner != null)
            spawner.OnEnemyDied(spawnIndex);

        // 1 saniye bekle (efekt için)
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
