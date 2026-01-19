using UnityEngine;


[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemDataSO : ScriptableObject
{
    [Header("Görsel Ayarlar")]
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Vector3 _size;


    [Header("İstatistikler")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _coolDown;
    [Header("Throw Settings")]
    public bool IsDiagonalThrow = false;

    public Sprite Sprite => _sprite;
    public float AttackDamage => _attackDamage;
    public float CoolDown => _coolDown;
    public Vector3 Size=>_size;
   
}
