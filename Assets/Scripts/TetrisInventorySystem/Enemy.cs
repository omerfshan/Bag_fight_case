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

    [SerializeField] private Animator anim;
    public int spawnIndex; // bu enemy hangi hedefe gidiyor
    private EnemySpawner spawner;

   public void Init(EnemySpawner spawnerRef, int index, Transform[] paths)
{
    spawner = spawnerRef;
    spawnIndex = index;
    setPlace = paths;
}
    void Awake()
    {
        spawner=FindAnyObjectByType<EnemySpawner>();
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
        yield return new WaitForSeconds(0.5f);

        
        damageText.gameObject.SetActive(false);
    }

   private void Die()
{
    spawner.OnEnemyDied(spawnIndex);  // spawner'a haber ver
    Destroy(gameObject);
}

    
}
