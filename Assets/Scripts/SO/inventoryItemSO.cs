using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
public class InventoryItemSO : ScriptableObject
{
    [Header("Görsel ve İsim")]
    [SerializeField] private string _itemName;

    [Header("GRID BOYUTU (Hücre olarak)")]
    [SerializeField] private int _width = 1;
    [SerializeField] private int _height = 1;

    [Header("UI BOYUTU (pixel veya scale)")]
    [SerializeField] private Vector2 _uiSize = new Vector2(100, 100);
    [SerializeField] private Vector3 _rotation;

    [Header("Item Shape (1 = dolu, 0 = boş)")]
    [SerializeField] private int[] _shape;

    [Header("ItemData SO (Statlar, Cooldown vs.)")]
    [SerializeField] private ItemDataSO _itemProperty;

    public string ItemName => _itemName;

    public int Width => _width;
    public int Height => _height;

    public Vector2 UISize => _uiSize;

    public int[] Shape => _shape;
    
    public Vector3 Rotation=>_rotation;
    public ItemDataSO ItemProperty => _itemProperty;

    
    public float CoolDown => _itemProperty != null ? _itemProperty.CoolDown : 0f;
    public float AttackDamage => _itemProperty != null ? _itemProperty.AttackDamage : 0f;
}
