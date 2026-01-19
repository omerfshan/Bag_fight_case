using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip enemyFootSteps;
    [SerializeField] private AudioClip enemyHurt;
    [SerializeField] private AudioClip enemyDie;

    [Header("Item Sounds")]
    [SerializeField] private AudioClip throwItem;
    [SerializeField] private AudioClip itemPick;
    [SerializeField] private AudioClip itemPlace;
     [SerializeField] private AudioClip trashSound;
     [SerializeField] private AudioClip itemSpawn;

    [Header("Volumes")]
    [SerializeField] private float enemyFootStepsVolume = 0.5f;
    [SerializeField] private float enemyHurtVolume = 0.5f;
    [SerializeField] private float enemyDieVolume = 0.5f;

    [SerializeField] private float throwItemVolume = 0.5f;
    [SerializeField] private float itemPickVolume = 0.5f;
    [SerializeField] private float itemPlaceVolume = 0.5f;
    [SerializeField] private float itemSpawnVolume = 0.5f;
    [SerializeField] private float trashSoundVolume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    // === ENEMY ===
    public void EnemyFootSound()
    {
        if (enemyFootSteps != null)
            audioSource.PlayOneShot(enemyFootSteps, enemyFootStepsVolume);
    }

    public void EnemyHurtSound()
    {
        if (enemyHurt != null)
            audioSource.PlayOneShot(enemyHurt, enemyHurtVolume);
    }

    public void EnemyDieSound()
    {
        if (enemyDie != null)
            audioSource.PlayOneShot(enemyDie, enemyDieVolume);
    }

    // === ITEM ===
    public void ThrowItemSound()
    {
        if (throwItem != null)
            audioSource.PlayOneShot(throwItem, throwItemVolume);
    }

    public void ItemPickSound()
    {
        if (itemPick != null)
            audioSource.PlayOneShot(itemPick, itemPickVolume);
    }

    public void ItemPlaceSound()
    {
        if (itemPlace != null)
            audioSource.PlayOneShot(itemPlace, itemPlaceVolume);
    }
    public void TrashSound()
    {
        if (trashSound != null)
            audioSource.PlayOneShot(trashSound, trashSoundVolume);
    }
    public void ItemSpawnSound()
    {
        if (itemSpawn != null)
            audioSource.PlayOneShot(itemSpawn, itemSpawnVolume);
    }

}
