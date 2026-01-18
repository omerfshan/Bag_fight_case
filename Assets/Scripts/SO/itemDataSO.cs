using UnityEngine;


[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemDataSO : ScriptableObject
{
    [Header("Görsel Ayarlar")]
    [SerializeField] private Sprite _sprite;


    [Header("İstatistikler")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _coolDown;

    public Sprite Sprite => _sprite;
    public float AttackDamage => _attackDamage;
    public float CoolDown => _coolDown;
   
}
