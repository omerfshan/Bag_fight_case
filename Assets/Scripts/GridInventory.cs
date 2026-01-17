using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridInventory : MonoBehaviour
{
    public Transform Grid;
    public List<CellSlot> cells = new List<CellSlot>();
    public List<GridItem> items = new List<GridItem>();

    void Awake()
    {
        if (!Grid) Grid = transform;
    }

    void Start()
    {
        if (cells.Count == 0 && Grid != null)
        {
            for (int i = 0; i < Grid.childCount; i++)
            {
                var slot = Grid.GetChild(i).GetComponent<CellSlot>();
                if (slot != null)
                    cells.Add(slot);
            }
        }
    }

    public void AddItem(GridItem item)
    {
        if (!items.Any(c => c.ID == item.ID))
        {
            item.isInInventory = true;
            items.Add(item);
        }
    }
}
