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
    public bool is_dead = false;   // ⭐ EKLENDİ

    [SerializeField] private Animator anim;
    public int spawnIndex;
    private EnemySpawner spawner;

    [SerializeField] private ParticleSystem[] DiedEfect;
    [SerializeField] private GameObject HealthParent;
    [SerializeField] private GameObject TextParent;

    private SpriteRenderer _renderer;

    public void Init(EnemySpawner spawnerRef, int index, Transform[] paths)
    {
        spawner = spawnerRef;
        spawnIndex = index;
        setPlace = paths;
    }

    void Awake()
    {
        spawner = FindAnyObjectByType<EnemySpawner>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        damageText.gameObject.SetActive(false);

        if (barFill != null)
            barFill.fillAmount = 1f;

        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Transform target = setPlace[spawnIndex];

        anim.SetBool(WalkID, true);

        while (Vector3.Distance(transform.position, target.position) > reachDistance)
        {
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
        if (is_dead) return; // ❗ Ölüyse tekrar hasar almasın

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
        StartCoroutine(DamageRoutine(dmg));

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

        damageText.gameObject.SetActive(true);
        damageText.text = "-" + dmg;
        yield return new WaitForEndOfFrame();

        anim.SetBool(HurtID, false);
        yield return new WaitForSeconds(0.1f);

        damageText.gameObject.SetActive(false);
    }

    // ============================================================================================
    // ⭐⭐ YENİ - ÖLÜM ANIMASYONU + PARTICLE + KONTROLLER ⭐⭐
    // ============================================================================================
    private void Die()
    {
        if (is_dead) return;
        is_dead = true;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Spawner'a haber ver (daha en başta)
      

        // UI kapat
        if (HealthParent != null) HealthParent.SetActive(false);
        if (TextParent != null) TextParent.SetActive(false);

        // Sprite kapat (yok olsun)
        if (_renderer != null) _renderer.enabled = false;

        // Particle efektleri başlat
        foreach (var p in DiedEfect)
        {
            if (p != null) p.Play();
        }
         spawner.OnEnemyDied(spawnIndex);
        // 1 saniye bekle (efektler gözüksün)
        yield return new WaitForSeconds(1f);
         
        Destroy(gameObject);
    }
}
